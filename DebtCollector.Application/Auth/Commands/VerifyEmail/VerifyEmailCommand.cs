using MediatR;

namespace DebtCollector.Application.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommand : IRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
