using debt_collector_api.Models;

namespace debt_collector_api.Responses
{
    public class GroupDTO : AuditInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<ExpenseDTO> Expenses { get; set; } = [];
        public List<PersonDTO> People { get; set; } = [];
    }
}
