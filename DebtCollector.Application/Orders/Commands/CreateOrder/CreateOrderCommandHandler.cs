using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var expense = await _context.Expenses
                .Include(e => e.Group)
                .Where(e => e.Id == request.ExpenseId)
                .FirstOrDefaultAsync(cancellationToken);

            if (expense == null) throw new KeyNotFoundException("Expense not found");

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId, cancellationToken);

            if (!isPersonInGroup) throw new UnauthorizedAccessException("Access denied");

            Order newOrder = new()
            {
                Id = request.Id, // Keeping client-provided ID support if SQL allows identity insert or is not identity
                ExpenseId = expense.Id,
                Name = request.Name,
                TotalCost = request.TotalCost,
                Debtors = [],
                Payers = [],
                CreatedOn = DateTime.UtcNow,
                CreatedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0,
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync(cancellationToken);

            var newPayers = request.Payers.Select(p => new Payer
            {
                PersonId = p.PersonId,
                OrderId = newOrder.Id,
                Value = p.Value,
                CreatedOn = DateTime.UtcNow,
                CreatedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0
            });

            var newDebtors = request.Debtors.Select(d => new Debtor
            {
                PersonId = d.PersonId,
                OrderId = newOrder.Id,
                Value = d.Value,
                CreatedOn = DateTime.UtcNow,
                CreatedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0
            });

            _context.Payers.AddRange(newPayers);
            _context.Debtors.AddRange(newDebtors);

            await _context.SaveChangesAsync(cancellationToken);

            // Recalculate Expense Total
            expense.TotalOrdersCost = await _context.Orders
                .Where(o => o.ExpenseId == expense.Id)
                .SumAsync(o => o.TotalCost, cancellationToken);
            
            await _context.SaveChangesAsync(cancellationToken);

            return newOrder;
        }
    }
}
