using DebtCollector.Application.DTOs;
using MediatR;

namespace DebtCollector.Application.Groups.Queries.GetGroupMembers
{
    public class GetGroupMembersQuery : IRequest<IEnumerable<PersonDTO>>
    {
        public int GroupId { get; set; }
    }
}
