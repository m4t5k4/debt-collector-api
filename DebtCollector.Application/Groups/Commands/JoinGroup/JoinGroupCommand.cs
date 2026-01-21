using MediatR;

namespace DebtCollector.Application.Groups.Commands.JoinGroup
{
    public class JoinGroupCommand : IRequest<string>
    {
        public string Password { get; set; } = string.Empty;
    }
}
