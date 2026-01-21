

namespace DebtCollector.Domain.Entities
{
    public class Order : AuditInfo
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }
        public List<Debtor> Debtors { get; set; } = [];
        public List<Payer> Payers { get; set; } = [];
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
    }
}
