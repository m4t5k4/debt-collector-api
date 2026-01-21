using DebtCollector.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public LogoutCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();

            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);

            if (person == null) throw new KeyNotFoundException("User not found");

            var refreshTokens = _context.RefreshTokens.Where(rt => rt.PersonId == personId);

            if (!refreshTokens.Any()) throw new KeyNotFoundException("No active refresh tokens found");

            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
