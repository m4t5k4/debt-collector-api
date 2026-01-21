using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Groups.Commands.CreateGroup
{
    public class CreateGroupCommand : IRequest<Group>
    {
        public string Name { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
    }
}
