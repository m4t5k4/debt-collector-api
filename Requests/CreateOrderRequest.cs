using debt_collector_api.Models;
using debt_collector_api.Responses;

namespace debt_collector_api.Requests
{
    public class CreateOrderRequest
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }
        public List<DebtorDTO> Debtors { get; set; } = [];
        public List<PayerDTO> Payers { get; set; } = [];
    }
}
