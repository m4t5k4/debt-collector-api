using System.Threading.Tasks;

namespace DebtCollector.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
