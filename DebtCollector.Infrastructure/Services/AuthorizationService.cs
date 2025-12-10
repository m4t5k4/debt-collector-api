using DebtCollector.Infrastructure.Persistence;
using DebtCollector.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DebtCollector.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DebtCollector.Infrastructure.Services
{
    public class AuthorizationService : ITokenService, ICurrentUserService
    {
        private readonly DebtCollectorContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AuthorizationService(
            DebtCollectorContext context,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration
            )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public int? GetCurrentPersonId()
        {
            string personIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(personIdClaim, out int personId) ? personId : null;
        }

        public async Task<string> GenerateUniqueRandomPasswordAsync(int length)
        {
            string password;
            bool isUnique = false;
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            do
            {
                password = new string(Enumerable.Range(0, length)
                                               .Select(_ => chars[random.Next(chars.Length)])
                                               .ToArray());

                isUnique = !await _context.Groups.AnyAsync(g => g.Password == password);
            }
            while (!isUnique);

            return password;
        }

        public string GenerateJwtToken(Person person)
        {
            byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt_Key"]);
            List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, person.Id.ToString()),
            new(ClaimTypes.Name, person.Username)
        };

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
