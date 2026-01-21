using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Groups.Queries.GetMyGroups
{
    public class GetMyGroupsQueryHandler : IRequestHandler<GetMyGroupsQuery, List<GroupDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyGroupsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<GroupDTO>> Handle(GetMyGroupsQuery request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException();

            var groups = await _context.PersonGroups
                .Where(pg => pg.PersonId == personId)
                .Include(pg => pg.Group)
                    .ThenInclude(g => g.Image)
                .Include(pg => pg.Group)
                    .ThenInclude(g => g.PersonGroups)
                        .ThenInclude(pg => pg.Person)
                .Select(pg => new GroupDTO
                {
                    Id = pg.Group.Id,
                    Name = pg.Group.Name,
                    Password = pg.Group.Password,
                    Image = pg.Group.Image,
                    CreatedOn = pg.Group.CreatedOn,
                    CreatedByPersonId = pg.Group.CreatedByPersonId,
                    ModifiedOn = pg.Group.ModifiedOn,
                    ModifiedByPersonId = pg.Group.ModifiedByPersonId,
                    People = pg.Group.PersonGroups.Select(pg => new PersonDTO
                    {
                        Id = pg.Person.Id,
                        Username = pg.Person.Username,
                        Image = pg.Person.Image,
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            if (groups == null || !groups.Any()) throw new KeyNotFoundException("You are not a member of any group.");

            return groups;
        }
    }
}
