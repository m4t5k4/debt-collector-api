using debt_collector_api.Models;

namespace debt_collector_api.Responses
{
    public class PersonDTO : AuditInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Image? Image { get; set; }
    }
}
