using Microsoft.EntityFrameworkCore;

namespace debt_collector_api.Models
{
    public class PaidBy : AuditInfo
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public int Value { get; set; }
    }
}
