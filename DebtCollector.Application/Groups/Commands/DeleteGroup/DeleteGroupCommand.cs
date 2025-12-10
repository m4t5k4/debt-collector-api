using MediatR;

namespace DebtCollector.Application.Groups.Commands.DeleteGroup
{
    public class DeleteGroupCommand : IRequest
    {
        public int Id { get; set; }
    }
}
