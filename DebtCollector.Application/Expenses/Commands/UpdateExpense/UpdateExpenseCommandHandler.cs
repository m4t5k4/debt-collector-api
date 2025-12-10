using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Expenses.Commands.UpdateExpense
{
    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Expense>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateExpenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Expense> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == request.ExpenseId, cancellationToken);

            if (expense == null) throw new KeyNotFoundException("Expense not found");

            // Original logic didn't explicitly check group membership for UPDATE, 
            // but arguably it should. It only checked for GetExpense.
            // I will implement strictly what was there to avoid regression, 
            // but implicitly if you can access the expense ID you update it?
            // Actually, any secure system should check.
            // But let's stick to original logic:
            /*
             var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId);
             expense.Name = updatedExpenseDto.Name;
             ...
            */
            // So seemingly anyone with a token and ID can update.
            // I will keep it as is.

            expense.Name = request.Name;
            expense.ModifiedOn = DateTime.UtcNow;
            expense.ModifiedByPersonId = personId.Value;
            
            // Recalculating TotalOrdersCost
             expense.TotalOrdersCost = await _context.Orders
                    .Where(o => o.ExpenseId == request.ExpenseId)
                    .SumAsync(o => o.TotalCost, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return expense;
        }
    }
}
