using MediatR;

namespace DebtCollector.Application.Groups.Commands.RemoveGroupMember
{
    public class RemoveGroupMemberCommand : IRequest
    {
        public int GroupId { get; set; }
        public int PersonId { get; set; }
    }
}
