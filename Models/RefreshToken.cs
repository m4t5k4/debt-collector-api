namespace debt_collector_api.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
