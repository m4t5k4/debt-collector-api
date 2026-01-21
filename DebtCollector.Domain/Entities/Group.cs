

namespace DebtCollector.Domain.Entities
{
    public class Group : AuditInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<PersonGroup> PersonGroups { get; set; } = [];
        public List<Expense> Expenses { get; set; } = [];
        public int? ImageId { get; set; }
        public Image? Image { get; set; }

    }
}
