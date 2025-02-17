using debt_collector_api.Models;

namespace debt_collector_api.Responses
{
    public class OrderDTO : AuditInfo
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public string Name { get; set; }
        public int TotalCost { get; set; }
        public List<DebtorDTO> Debtors { get; set; } = [];
        public List<PayerDTO> Payers { get; set; } = [];
    }
}
