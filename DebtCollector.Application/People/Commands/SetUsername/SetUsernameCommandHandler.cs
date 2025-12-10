using DebtCollector.Application.Common.Interfaces;
using MediatR;

namespace DebtCollector.Application.People.Commands.SetUsername
{
    public class SetUsernameCommandHandler : IRequestHandler<SetUsernameCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SetUsernameCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(SetUsernameCommand request, CancellationToken cancellationToken)
        {
            var currentPersonId = _currentUserService.GetCurrentPersonId();

            if (currentPersonId == null || currentPersonId != request.PersonId)
                throw new UnauthorizedAccessException("Invalid token");

            var person = await _context.Persons.FindAsync(new object[] { request.PersonId }, cancellationToken);
            if (person == null) throw new KeyNotFoundException("Person not found");

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username cannot be empty.");

            person.Username = request.Username;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
