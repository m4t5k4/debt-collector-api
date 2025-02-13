using Microsoft.EntityFrameworkCore;

namespace debt_collector_api.Models
{
    public class Order : AuditInfo
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public string Name { get; set; }
        public int TotalCost { get; set; }
        public List<Debtor> Debtors { get; set; } = [];
        public List<Payer> Payers { get; set; } = [];
    }
}
