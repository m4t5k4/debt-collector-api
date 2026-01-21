using DebtCollector.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeleteOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var order = await _context.Orders
                .Include(o => o.Payers)
                .Include(o => o.Debtors)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null) throw new KeyNotFoundException("Order not found");

            _context.Payers.RemoveRange(order.Payers);
            _context.Debtors.RemoveRange(order.Debtors);
            _context.Orders.Remove(order);
            
            // Recalculate (Need to save removal first? Or standard EF handles it if I recalculate after? 
            // Original code: remove, then recalculate.
            // Recalculate sums total cost of orders with expenseId. 
            // If I haven't saved changes, the deleted order is still in DB conceptually or tracked as deleted?
            // EF Core SumAsync on DbSet executes in DB. If I haven't saved, DB still has the order.
            // So logic needs: Remove -> Save (so DB is updated) -> Recalculate -> Save.
            // Wait, original code:
            /*
            _context.Orders.Remove(order);
            await RecalculateExpenseTotalAsync(order.Id); // This calls context.Orders...SumAsync
            await _context.SaveChangesAsync();
            */
            // If they didn't save before recalculating, the SumAsync would include the deleted order unless EF Core applies local changes to the query?
            // EF Core executes raw SQL for SumAsync usually if on DbSet, effectively seeing DB state?
            // Or does it mix? `_context.Orders.SumAsync` usually runs on DB.
            // If DB has the order, sum includes it.
            // So `RecalculateExpenseTotalAsync` in original code might have been BUGGY if not saved!
            // BUT: `RecalculateExpenseTotalAsync` calculates based on `expense.Id`.
            // Let's verify if `Remove` marks it deleted and subsequent queries exclude it?
            // Generally NO, unless Local is used.
            // I will fix this potential bug by doing: remove -> save -> recalculate -> save.

            int expenseId = order.ExpenseId;

            await _context.SaveChangesAsync(cancellationToken); 

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);
            if (expense != null)
            {
                expense.TotalOrdersCost = await _context.Orders
                    .Where(o => o.ExpenseId == expenseId)
                    .SumAsync(o => o.TotalCost, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
