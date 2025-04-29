using debt_collector_api.Data;
using debt_collector_api.Helpers;
using debt_collector_api.Models;
using debt_collector_api.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly AuthorizationHelper _authorizationHelper;

        public AuthController(
            DebtCollectorContext context,
            AuthorizationHelper authorizationHelper
            )
        {
            _context = context;
            _authorizationHelper = authorizationHelper;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { message = "Invalid request" });

            var person = await _context.Persons
                .Where(p => p.Email == model.Email)
                .FirstOrDefaultAsync();

            if (person == null) return Unauthorized(new { message = "Invalid user" });

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, person.Password);

            if (!isPasswordCorrect) return Unauthorized(new { message = "Invalid password" });

            var token = _authorizationHelper.GenerateJwtToken(person);

            var refreshToken = _authorizationHelper.GenerateRefreshToken();

            var storedToken = new RefreshToken
            {
                PersonId = person.Id,
                Token = refreshToken,
                Expiration = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(storedToken);
            await _context.SaveChangesAsync();

            return Ok(new { token, refreshToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = _authorizationHelper.GetCurrentPersonId();

            if (userId == null) return Unauthorized(new { message = "Invalid token" });

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == userId);

            if (person == null) return NotFound(new { message = "User not found" });

            var refreshTokens = _context.RefreshTokens.Where(rt => rt.PersonId == userId);

            if (!refreshTokens.Any()) return NotFound(new { message = "No active refresh tokens found" });

            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest(new { message = "Invalid refresh token request" });

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (storedToken == null || storedToken.Expiration < DateTime.UtcNow)
                return Unauthorized(new { message = "Refresh token is invalid or expired" });

            var person = await _context.Persons.FindAsync(storedToken.PersonId);

            if (person == null) return Unauthorized(new { message = "User not found" });

            var newJwt = _authorizationHelper.GenerateJwtToken(person);
            var newRefreshToken = _authorizationHelper.GenerateRefreshToken();

            storedToken.Token = newRefreshToken;
            storedToken.Expiration = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new { token = newJwt, refreshToken = newRefreshToken });
        }
    }
}
