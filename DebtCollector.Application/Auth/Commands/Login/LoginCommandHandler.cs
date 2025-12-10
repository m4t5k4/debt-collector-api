using DebtCollector.Application.Auth.DTOs;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
                .Where(p => p.Email == request.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (person == null)
            {
                throw new UnauthorizedAccessException("Invalid user");
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, person.Password);

            if (!isPasswordCorrect)
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            var token = _tokenService.GenerateJwtToken(person);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var storedToken = new RefreshToken
            {
                PersonId = person.Id,
                Token = refreshToken,
                Expiration = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(storedToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResult
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}
