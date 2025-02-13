namespace debt_collector_api.Models
{
    public class Debtor
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Value { get; set; }
        public string Name { get; set; }
        public bool HasPaid { get; set; }
    }
}
