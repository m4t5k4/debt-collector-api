using Microsoft.EntityFrameworkCore;

namespace debt_collector_api.Models
{
    public class Person : AuditInfo
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<PersonGroup> PersonGroups { get; set; } = [];
        public List<Order> Orders { get; set; } = [];
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
        public string PrimaryCurrency { get; set; } = "EUR";
    }
}
