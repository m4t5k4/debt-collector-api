using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.DTOs
{
    public class PayerDTO : AuditInfo
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int OrderId { get; set; }
        public decimal Value { get; set; }
    }
}
