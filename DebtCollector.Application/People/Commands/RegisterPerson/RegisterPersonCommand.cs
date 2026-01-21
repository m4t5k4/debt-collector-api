using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.People.Commands.RegisterPerson
{
    public class RegisterPersonCommand : IRequest<Person>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
