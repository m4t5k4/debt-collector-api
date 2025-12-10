using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Expense>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateExpenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Expense> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var group = await _context.Groups.FindAsync(new object[] { request.GroupId }, cancellationToken);

            if (group == null) throw new KeyNotFoundException("Invalid GroupId, group does not exist.");

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(gu => gu.GroupId == request.GroupId && gu.PersonId == personId, cancellationToken);

            if (!isPersonInGroup) throw new UnauthorizedAccessException("You are not a member of this group.");
            // Original controller returned Forbid() which is 403. UnauthorizedAccessException maps to 401 usually.
            // I should stick to consistent exceptions. UnauthorizedAccessException is fine for now, controller can map it.

            var expense = new Expense
            {
                GroupId = request.GroupId,
                Name = request.Name,
                Currency = request.Currency,
                TotalOrdersCost = 0,
                CreatedByPersonId = personId.Value,
                ModifiedByPersonId = personId.Value,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync(cancellationToken);

            return expense;
        }
    }
}
