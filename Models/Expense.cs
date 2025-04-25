using Microsoft.EntityFrameworkCore;

namespace debt_collector_api.Models
{
    public class Expense : AuditInfo
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string Name { get; set; }
        public List<Order> Orders { get; set; } = [];
        public string Currency { get; set; } = "EUR";
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
        public decimal TotalOrdersCost { get; set; }
    }
}
