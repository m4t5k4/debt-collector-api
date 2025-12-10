using MediatR;
using DebtCollector.Application.Groups.Queries.GetGroups;
using DebtCollector.Application.DTOs;
// HotChocolate injection attribute
using HotChocolate;

namespace DebtCollector.Api.GraphQL
{
    public class Query
    {
        public async Task<IEnumerable<GroupDTO>> GetGroups([Service] ISender sender)
        {
            return await sender.Send(new GetGroupsQuery());
        }
    }
}
