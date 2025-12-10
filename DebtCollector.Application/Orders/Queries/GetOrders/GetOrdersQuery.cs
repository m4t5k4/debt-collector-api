using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Orders.Queries.GetOrders
{
    public class GetOrdersQuery : IRequest<IEnumerable<Order>>
    {
    }
}
