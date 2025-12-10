using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.Images.Commands.UploadAvatar
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, ImageDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBlobStorageService _blobStorageService;

        public UploadAvatarCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _blobStorageService = blobStorageService;
        }

        public async Task<ImageDTO> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            if (request.FileBytes == null || request.FileBytes.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(request.ContentType.ToLower()))
                throw new ArgumentException("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{System.IO.Path.GetFileNameWithoutExtension(request.FileName)}_{timestamp}{System.IO.Path.GetExtension(request.FileName)}";

            using var fileStream = new MemoryStream(request.FileBytes);
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var person = await _context.Persons.FindAsync(new object[] { personId }, cancellationToken);
            if (person == null) throw new KeyNotFoundException("Person not found");

            // Delete old image if exists and not placeholder
            var oldImage = await _context.Images.FindAsync(new object[] { person.ImageId }, cancellationToken);
            if (oldImage != null && !oldImage.Url.Contains("avatar_placeholder.png"))
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            var image = new Image { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync(cancellationToken);

            person.ImageId = image.Id;
            await _context.SaveChangesAsync(cancellationToken);

            return new ImageDTO { Id = image.Id, Url = image.Url };
        }
    }
}
