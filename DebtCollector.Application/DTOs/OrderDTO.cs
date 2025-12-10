using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.DTOs
{
    public class OrderDTO : AuditInfo
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }
        public List<DebtorDTO> Debtors { get; set; } = [];
        public List<PayerDTO> Payers { get; set; } = [];
    }
}
