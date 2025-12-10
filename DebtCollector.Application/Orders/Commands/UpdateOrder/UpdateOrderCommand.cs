using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<Order>
    {
        public int Id { get; set; }
        public OrderDTO OrderDto { get; set; } = null!;
    }
}
