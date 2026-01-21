using MediatR;

namespace DebtCollector.Application.People.Commands.SetUsername
{
    public class SetUsernameCommand : IRequest
    {
        public int PersonId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
