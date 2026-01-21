using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<Order>
    {
        public int Id { get; set; }
    }
}
