using DebtCollector.Application.Auth.DTOs;
using MediatR;

namespace DebtCollector.Application.Auth.Commands.Refresh
{
    public class RefreshCommand : IRequest<AuthResult>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
