using DebtCollector.Application.Auth.DTOs;
using DebtCollector.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public RefreshCommandHandler(IApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
             if (string.IsNullOrEmpty(request.RefreshToken))
                throw new ArgumentException("Invalid refresh token request");

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

            if (storedToken == null || storedToken.Expiration < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token is invalid or expired");

            var person = await _context.Persons.FindAsync(new object[] { storedToken.PersonId }, cancellationToken);

            if (person == null) throw new UnauthorizedAccessException("User not found");

            var newJwt = _tokenService.GenerateJwtToken(person);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            storedToken.Token = newRefreshToken;
            storedToken.Expiration = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResult { Token = newJwt, RefreshToken = newRefreshToken };
        }
    }
}
