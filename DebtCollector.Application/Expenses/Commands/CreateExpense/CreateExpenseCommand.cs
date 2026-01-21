using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommand : IRequest<Expense>
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }
}
