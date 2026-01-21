using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.DTOs
{
    public class GroupDTO : AuditInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<ExpenseDTO> Expenses { get; set; } = [];
        public List<PersonDTO> People { get; set; } = [];
        public Image? Image { get; set; }
    }
}
