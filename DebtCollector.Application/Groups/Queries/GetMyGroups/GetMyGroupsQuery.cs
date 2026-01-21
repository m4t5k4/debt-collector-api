using DebtCollector.Application.DTOs;
using MediatR;

namespace DebtCollector.Application.Groups.Queries.GetMyGroups
{
    public class GetMyGroupsQuery : IRequest<List<GroupDTO>>
    {
    }
}
