using DebtCollector.Application.Common.Interfaces;
using MediatR;

namespace DebtCollector.Application.People.Commands.SetCurrency
{
    public class SetCurrencyCommandHandler : IRequestHandler<SetCurrencyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SetCurrencyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(SetCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currentPersonId = _currentUserService.GetCurrentPersonId();

            if (currentPersonId == null || currentPersonId != request.PersonId)
                throw new UnauthorizedAccessException("Invalid token");

            var person = await _context.Persons.FindAsync(new object[] { request.PersonId }, cancellationToken);
            if (person == null) throw new KeyNotFoundException("Person not found");

            if (string.IsNullOrWhiteSpace(request.Currency))
                throw new ArgumentException("Currency cannot be empty.");

            person.PrimaryCurrency = request.Currency;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
