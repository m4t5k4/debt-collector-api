using DebtCollector.Application.DTOs;
using MediatR;

namespace DebtCollector.Application.Groups.Queries.GetGroupById
{
    public class GetGroupByIdQuery : IRequest<GroupDTO>
    {
        public int Id { get; set; }
    }
}
