using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Expenses.Queries.GetExpenseById
{
    public class GetExpenseByIdQuery : IRequest<Expense>
    {
        public int Id { get; set; }
    }
}
