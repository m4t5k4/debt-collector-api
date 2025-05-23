using debt_collector_api.Data;
using debt_collector_api.Helpers;
using debt_collector_api.Models;
using debt_collector_api.Requests;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthController(
            DebtCollectorContext context,
            AuthorizationHelper authorizationHelper,
            IConfiguration configuration,
            IEmailService emailService
            )
        {
            _context = context;
            _authorizationHelper = authorizationHelper;
            _configuration = configuration;
            _emailService = emailService;
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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == personId);

            if (person == null) return NotFound(new { message = "User not found" });

            var refreshTokens = _context.RefreshTokens.Where(rt => rt.PersonId == personId);

            if (!refreshTokens.Any()) return NotFound(new { message = "No active refresh tokens found" });

            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh")]
        [Authorize]
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

        [HttpPost("send-verification-email")]
        [Authorize]
        public async Task<IActionResult> SendVerificationEmail()
        {
            var personId = _authorizationHelper.GetCurrentPersonId();
            if (personId == null) return Unauthorized();

            var person = await _context.Persons.FindAsync(personId);
            if (person == null || string.IsNullOrEmpty(person.Email))
                return BadRequest(new { message = "Invalid user or email." });

            var code = GenerateVerificationCode();

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "VerificationEmail.html");
            var body = EmailTemplateHelper.GetTemplate(templatePath, new Dictionary<string, string>
            {
                { "{{CODE}}", code }
            });

            var subject = "Jouw verificatiecode";

            await _emailService.SendEmailAsync(person.Email, subject, body);

            person.EmailVerificationToken = code;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("verify-email")]
        [Authorize]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            if (request.Token == null) return BadRequest(new { message = "Invalid or expired token." });

            var personId = _authorizationHelper.GetCurrentPersonId();
            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var person = await _context.Persons.FindAsync(personId);
            if (person == null) return NotFound(new { message = "User not found" });

            if (request.Token == person.EmailVerificationToken)
            {
                person.EmailVerificationToken = null;
                person.EmailVerifiedAt = DateTime.UtcNow;
                person.IsVerified = true;
                await _context.SaveChangesAsync();
            } else return BadRequest(new { message = "Invalid or expired token." });

            return Ok(new { message = "Email verified successfully." });
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
