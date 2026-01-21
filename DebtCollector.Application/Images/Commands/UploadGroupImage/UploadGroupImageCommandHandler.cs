using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Images.Commands.UploadGroupImage
{
    public class UploadGroupImageCommandHandler : IRequestHandler<UploadGroupImageCommand, ImageDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBlobStorageService _blobStorageService;

        public UploadGroupImageCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _blobStorageService = blobStorageService;
        }

        public async Task<ImageDTO> Handle(UploadGroupImageCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            if (request.FileBytes == null || request.FileBytes.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(request.ContentType.ToLower()))
                throw new ArgumentException("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var group = await _context.Groups.FindAsync(new object[] { request.GroupId }, cancellationToken);
            if (group == null) throw new KeyNotFoundException("Group not found.");

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == group.Id, cancellationToken);

            if (!isPersonInGroup) throw new UnauthorizedAccessException("Access denied");

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{System.IO.Path.GetFileNameWithoutExtension(request.FileName)}_{timestamp}{System.IO.Path.GetExtension(request.FileName)}";

            using var fileStream = new MemoryStream(request.FileBytes);
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var oldImage = await _context.Images.FindAsync(new object[] { group.ImageId }, cancellationToken);
            if (oldImage != null)
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            var image = new Image { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync(cancellationToken);

            group.ImageId = image.Id;
            await _context.SaveChangesAsync(cancellationToken);

            return new ImageDTO { Id = image.Id, Url = image.Url };
        }
    }
}
