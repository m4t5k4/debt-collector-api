using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Groups.Commands.CreateGroup
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Group>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;
        private readonly IBlobStorageService _blobStorageService;

        public CreateGroupCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, ITokenService tokenService, IBlobStorageService blobStorageService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
            _blobStorageService = blobStorageService;
        }

        public async Task<Group> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            Image? image = null;
            if (request.Image != null && request.Image.Length > 0)
            {
                 string fileName = $"group_{Guid.NewGuid()}.jpg"; 
                 using var stream = new MemoryStream(request.Image);
                 var url = await _blobStorageService.UploadImageAsync(fileName, stream);
                 
                 image = new Image { Url = url };
                 _context.Images.Add(image);
                 await _context.SaveChangesAsync(cancellationToken);
            }

            var group = new Group
            {
                Name = request.Name,
                ImageId = image?.Id, // Set ImageId if image exists
                // Image property will be set by EF Core navigation fixup or explicitly if we want return to include it
                Image = image, 
                CreatedByPersonId = personId.Value,
                ModifiedByPersonId = personId.Value,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Password = await _tokenService.GenerateUniqueRandomPasswordAsync(5)
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync(cancellationToken);

            var personGroup = new PersonGroup
            {
                PersonId = personId.Value,
                GroupId = group.Id
            };

            _context.PersonGroups.Add(personGroup);
            await _context.SaveChangesAsync(cancellationToken);

            return group;
        }
    }
}
