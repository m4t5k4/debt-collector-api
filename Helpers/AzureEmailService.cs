using Azure.Communication.Email;
using Azure;

namespace debt_collector_api.Helpers
{
    public class AzureEmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _fromEmail;

        public AzureEmailService(IConfiguration config)
        {
            var connectionString = config["AzureEmail:ConnectionString"];
            _fromEmail = config["AzureEmail:From"];
            _emailClient = new EmailClient(connectionString);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailContent = new EmailContent(subject)
            {
                Html = body
            };

            var recipients = new EmailRecipients(new[]
            {
                new EmailAddress(to)
            });

            var message = new EmailMessage(_fromEmail, recipients, emailContent);

            try
            {
                await _emailClient.SendAsync(WaitUntil.Completed, message);
            }
            catch (Exception ex)
            {
                // Log or handle the error appropriately
                throw new ApplicationException("Failed to send email via Azure Communication Services", ex);
            }
        }
    }
}
