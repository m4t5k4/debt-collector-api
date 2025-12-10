using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.DTOs
{
    public class ExpenseDTO : AuditInfo
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public List<OrderDTO> Orders { get; set; } = [];
        public string Currency { get; set; } = "EUR";
        public Image? Image { get; set; }
        public decimal TotalOrdersCost { get; set; }
    }
}
