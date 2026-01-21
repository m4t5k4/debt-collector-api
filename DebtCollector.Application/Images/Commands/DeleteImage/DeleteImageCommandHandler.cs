using DebtCollector.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Images.Commands.DeleteImage
{
    public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBlobStorageService _blobStorageService;

        public DeleteImageCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _blobStorageService = blobStorageService;
        }

        public async Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var image = await _context.Images.FindAsync(new object[] { request.ImageId }, cancellationToken);
            if (image == null) throw new KeyNotFoundException("Image not found");

            try
            {
                await _blobStorageService.DeleteImageAsync(image.Url);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to delete image from storage");
            }

            _context.Images.Remove(image);

            if (!string.IsNullOrEmpty(request.ResourceType))
            {
                switch (request.ResourceType.ToLower())
                {
                    case "avatar":
                        var person = await _context.Persons.FirstOrDefaultAsync(p => p.ImageId == request.ImageId, cancellationToken);
                        if (person != null)
                        {
                            person.ImageId = null;
                            _context.Persons.Update(person);
                        }
                        break;

                    case "expense":
                        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.ImageId == request.ImageId, cancellationToken);
                        if (expense != null)
                        {
                            expense.ImageId = null;
                            _context.Expenses.Update(expense);
                        }
                        break;

                    case "order":
                        var order = await _context.Orders.FirstOrDefaultAsync(o => o.ImageId == request.ImageId, cancellationToken);
                        if (order != null)
                        {
                            order.ImageId = null;
                            _context.Orders.Update(order);
                        }
                        break;

                    case "group":
                        var group = await _context.Groups.FirstOrDefaultAsync(g => g.ImageId == request.ImageId, cancellationToken);
                        if (group != null)
                        {
                            group.ImageId = null;
                            _context.Groups.Update(group);
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid resource type");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
