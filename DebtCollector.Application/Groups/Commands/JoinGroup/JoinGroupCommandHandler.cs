using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

// Note: SignalR notification logic is currently in the Controller. 
// Ideally, we publish a Domain Event or Integration Event here, 
// and a notification handler sends the SignalR message.
// For now, we will return the GroupId as string to allow the Controller to send the notification if we want to keep it simple,
// or we can inject IMetadata/HubContext if we move Hub logic. 
// But Application layer shouldn't depend on SignalR Hubs directly usually.
// I'll stick to returning the GroupId so the controller can do the SignalR part for now (Hybrid approach),
// OR better: Return a Result object.

namespace DebtCollector.Application.Groups.Commands.JoinGroup
{
    public class JoinGroupCommandHandler : IRequestHandler<JoinGroupCommand, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public JoinGroupCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(JoinGroupCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("User not authenticated");

            var group = await _context.Groups
                .Where(g => g.Password.ToLower() == request.Password.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (group == null) throw new KeyNotFoundException("Group with this password not found");

            var existingMember = await _context.PersonGroups
                .Where(pg => pg.PersonId == personId && pg.GroupId == group.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingMember != null) throw new InvalidOperationException("You are already a member of this group");

            var personGroup = new PersonGroup
            {
                PersonId = personId.Value,
                GroupId = group.Id
            };

            _context.PersonGroups.Add(personGroup);
            await _context.SaveChangesAsync(cancellationToken);

            return group.Id.ToString();
        }
    }
}
