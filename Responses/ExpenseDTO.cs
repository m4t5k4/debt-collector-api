using debt_collector_api.Models;

namespace debt_collector_api.Responses
{
    public class ExpenseDTO : AuditInfo
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public List<OrderDTO> Orders { get; set; } = [];
        public string Currency { get; set; } = "EUR";
        public Image? Image { get; set; }
    }
}
