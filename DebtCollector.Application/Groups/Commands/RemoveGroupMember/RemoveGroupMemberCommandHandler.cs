using DebtCollector.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Groups.Commands.RemoveGroupMember
{
    public class RemoveGroupMemberCommandHandler : IRequestHandler<RemoveGroupMemberCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RemoveGroupMemberCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var currentPersonId = _currentUserService.GetCurrentPersonId();
            if (currentPersonId == null) throw new UnauthorizedAccessException("Invalid token");

            var group = await _context.Groups
                .Include(g => g.PersonGroups)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

            if (group == null) throw new KeyNotFoundException("Group not found");

            var personGroup = group.PersonGroups.FirstOrDefault(pg => pg.PersonId == request.PersonId);
            if (personGroup == null) throw new KeyNotFoundException("Member not found in this group");

            _context.PersonGroups.Remove(personGroup);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
