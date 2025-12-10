using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Expenses.Commands.UpdateExpense
{
    public class UpdateExpenseCommand : IRequest<Expense>
    {
        public int ExpenseId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
