using MediatR;
using Microsoft.EntityFrameworkCore;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.Groups.Queries.GetGroups
{
    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, IEnumerable<GroupDTO>>
    {
        private readonly IApplicationDbContext _context;

        public GetGroupsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GroupDTO>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            // Note: This logic is based on the legacy controller but adapted for CQRS.
            // Ideally, we should use AutoMapper, but for now manual mapping as per legacy code.
            // Also, the legacy GetGroup returned Entity, but GetGroupsForCurrentUser returned DTO.
            // The Controller 'GetGroup' (singular but list) returned List<Group> entities.
            // I will implement returning GroupDTOs to be cleaner.
            
            var groups = await _context.Groups.ToListAsync(cancellationToken);

            return groups.Select(g => new GroupDTO
            {
                Id = g.Id,
                Name = g.Name,
                // Map other properties as needed
                CreatedOn = g.CreatedOn,
                CreatedByPersonId = g.CreatedByPersonId,
                ModifiedOn = g.ModifiedOn,
                ModifiedByPersonId = g.ModifiedByPersonId,
                // Note: Legacy DTO might need Password or ImageUrl mapping
            });
        }
    }
}
