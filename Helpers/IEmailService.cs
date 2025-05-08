namespace debt_collector_api.Helpers
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
