using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Images.Commands.UploadExpenseImage
{
    public class UploadExpenseImageCommandHandler : IRequestHandler<UploadExpenseImageCommand, ImageDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBlobStorageService _blobStorageService;

        public UploadExpenseImageCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _blobStorageService = blobStorageService;
        }

        public async Task<ImageDTO> Handle(UploadExpenseImageCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            if (request.FileBytes == null || request.FileBytes.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(request.ContentType.ToLower()))
                throw new ArgumentException("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var expense = await _context.Expenses.FindAsync(new object[] { request.ExpenseId }, cancellationToken);
            if (expense == null) throw new KeyNotFoundException("Expense not found.");

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId, cancellationToken);

            if (!isPersonInGroup) throw new UnauthorizedAccessException("Access denied");

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{System.IO.Path.GetFileNameWithoutExtension(request.FileName)}_{timestamp}{System.IO.Path.GetExtension(request.FileName)}";

            using var fileStream = new MemoryStream(request.FileBytes);
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var oldImage = await _context.Images.FindAsync(new object[] { expense.ImageId }, cancellationToken);
            if (oldImage != null)
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            var image = new Image { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync(cancellationToken);

            expense.ImageId = image.Id;
            await _context.SaveChangesAsync(cancellationToken);

            return new ImageDTO { Id = image.Id, Url = image.Url };
        }
    }
}
