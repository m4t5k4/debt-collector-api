using MediatR;
using DebtCollector.Application.DTOs;

namespace DebtCollector.Application.Groups.Queries.GetGroups
{
    public class GetGroupsQuery : IRequest<IEnumerable<GroupDTO>>
    {
    }
}
