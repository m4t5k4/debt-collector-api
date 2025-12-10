using DebtCollector.Application.Auth.DTOs;
using MediatR;

namespace DebtCollector.Application.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AuthResult>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
