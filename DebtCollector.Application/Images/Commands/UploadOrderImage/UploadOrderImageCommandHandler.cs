using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Images.Commands.UploadOrderImage
{
    public class UploadOrderImageCommandHandler : IRequestHandler<UploadOrderImageCommand, ImageDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBlobStorageService _blobStorageService;

        public UploadOrderImageCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _blobStorageService = blobStorageService;
        }

        public async Task<ImageDTO> Handle(UploadOrderImageCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            if (request.FileBytes == null || request.FileBytes.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(request.ContentType.ToLower()))
                throw new ArgumentException("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
            if (order == null) throw new KeyNotFoundException("Order not found.");

            var expense = await _context.Expenses.FindAsync(new object[] { order.ExpenseId }, cancellationToken);
            if (expense == null) throw new KeyNotFoundException("Expense not found");

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId, cancellationToken);

            if (!isPersonInGroup) throw new UnauthorizedAccessException("Access denied");

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{System.IO.Path.GetFileNameWithoutExtension(request.FileName)}_{timestamp}{System.IO.Path.GetExtension(request.FileName)}";

            using var fileStream = new MemoryStream(request.FileBytes);
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var oldImage = await _context.Images.FindAsync(new object[] { order.ImageId }, cancellationToken);
            if (oldImage != null)
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            var image = new Image { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync(cancellationToken);

            order.ImageId = image.Id;
            await _context.SaveChangesAsync(cancellationToken);

            return new ImageDTO { Id = image.Id, Url = image.Url };
        }
    }
}
