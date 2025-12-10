using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Groups.Queries.GetGroupMembers
{
    public class GetGroupMembersQueryHandler : IRequestHandler<GetGroupMembersQuery, IEnumerable<PersonDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetGroupMembersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<PersonDTO>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException();

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == request.GroupId, cancellationToken);

            // Using UnauthorizedAccessException for Forbid equivalence here roughly
            if (!isPersonInGroup) throw new UnauthorizedAccessException("Access denied to this group");

            var people = await _context.PersonGroups
                .Where(pg => pg.GroupId == request.GroupId)
                .Include(pg => pg.Person)
                .Select(pg => new PersonDTO
                {
                    Id = pg.Person.Id,
                    Username = pg.Person.Username,
                })
                .ToListAsync(cancellationToken);

            return people;
        }
    }
}
