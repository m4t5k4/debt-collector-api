

namespace DebtCollector.Domain.Entities
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
        public bool IsVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationSentAt { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
    }
}
