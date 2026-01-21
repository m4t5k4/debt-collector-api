namespace DebtCollector.Domain.Entities
{
    public class Debtor : AuditInfo
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Value { get; set; }
        public bool HasPaid { get; set; }
    }
}
