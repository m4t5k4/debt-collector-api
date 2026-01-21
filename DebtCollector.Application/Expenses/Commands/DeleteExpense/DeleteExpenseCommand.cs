using MediatR;

namespace DebtCollector.Application.Expenses.Commands.DeleteExpense
{
    public class DeleteExpenseCommand : IRequest
    {
        public int Id { get; set; }
    }
}
