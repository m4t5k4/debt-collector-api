using DebtCollector.Application.Common.Interfaces;
using MediatR;

namespace DebtCollector.Application.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public VerifyEmailCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
             if (request.Token == null) throw new ArgumentException("Invalid or expired token.");

            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var person = await _context.Persons.FindAsync(new object[] { personId }, cancellationToken);
            if (person == null) throw new KeyNotFoundException("User not found");

            if (request.Token == person.EmailVerificationToken)
            {
                person.EmailVerificationToken = null;
                person.EmailVerifiedAt = DateTime.UtcNow;
                person.IsVerified = true;
                await _context.SaveChangesAsync(cancellationToken);
            } else 
            {
                throw new ArgumentException("Invalid or expired token.");
            }
        }
    }
}
