using debt_collector_api.Data;
using debt_collector_api.Models;
using debt_collector_api.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(DebtCollectorContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { message = "Invalid request" });
            }

            var person = await _context.Persons
                .Where(p => p.Username == model.Username)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, person.Password);

            if (!isPasswordCorrect)
            {
                return Unauthorized(new { message = "Invalid password" });
            }

            var token = GenerateJwtToken(person);

            var refreshToken = GenerateRefreshToken();

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
            var userId = GetCurrentPersonId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == userId);

            if (person == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var refreshTokens = _context.RefreshTokens.Where(rt => rt.PersonId == userId);

            if (!refreshTokens.Any())
            {
                return NotFound(new { message = "No active refresh tokens found" });
            }

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

            var newJwt = GenerateJwtToken(person);
            var newRefreshToken = GenerateRefreshToken();

            storedToken.Token = newRefreshToken;
            storedToken.Expiration = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new { token = newJwt, refreshToken = newRefreshToken });
        }

        private int? GetCurrentPersonId()
        {
            var nameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (nameIdentifierClaim != null)
            {
                return int.Parse(nameIdentifierClaim.Value);
            }

            return null;
        }

        private string GenerateJwtToken(Person person)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt_Key"]);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, person.Id.ToString()),
            new Claim(ClaimTypes.Name, person.Username)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
