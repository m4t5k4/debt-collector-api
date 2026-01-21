using DebtCollector.Application.Common.Interfaces;
using MediatR;

namespace DebtCollector.Application.Expenses.Commands.DeleteExpense
{
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeleteExpenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var expense = await _context.Expenses.FindAsync(new object[] { request.Id }, cancellationToken);
            if (expense == null) throw new KeyNotFoundException("Expense not found");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
