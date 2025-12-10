using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public List<PayerDTO> Payers { get; set; } = [];
        public List<DebtorDTO> Debtors { get; set; } = [];
    }
}
