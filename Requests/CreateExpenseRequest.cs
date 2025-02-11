namespace debt_collector_api.Requests
{
    public class CreateExpenseRequest
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; } = "EUR";
    }
}
