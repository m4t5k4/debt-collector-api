using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.DTOs
{
    public class PersonDTO : AuditInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Image? Image { get; set; }
        public string PrimaryCurrency { get; set; }
        public bool IsVerified { get; set; }
        public decimal Balance { get; set; }
    }
}
