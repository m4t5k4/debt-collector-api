namespace debt_collector_api.Models
{
    public class AuditInfo
    {
        public int CreatedByPersonId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedByPersonId { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
