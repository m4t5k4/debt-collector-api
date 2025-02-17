using debt_collector_api.Models;

namespace debt_collector_api.Responses
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
