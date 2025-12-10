using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Expenses.Queries.GetExpenseById
{
    public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, Expense>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetExpenseByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Expense> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var expense = await _context.Expenses
                .Include(e => e.Orders)
                .ThenInclude(o => o.Image)
                .Include(e => e.Image)
                .Where(e => e.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (expense == null) throw new KeyNotFoundException("Expense not found");

            // Authorization check
            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId, cancellationToken);

            if (!isPersonInGroup) throw new UnauthorizedAccessException("Access denied");

            return expense;
        }
    }
}
