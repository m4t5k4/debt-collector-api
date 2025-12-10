using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.DTOs
{
    public class DebtorDTO : AuditInfo
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int OrderId { get; set; }
        public decimal Value { get; set; }
        public bool HasPaid { get; set; }
    }
}
