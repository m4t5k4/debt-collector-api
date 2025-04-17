using debt_collector_api.Data;
using debt_collector_api.Helpers;
using debt_collector_api.Models;
using debt_collector_api.Requests;
using debt_collector_api.Responses;
using debt_collector_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly AuthorizationHelper _authorizationHelper;
        private readonly BlobStorageService _blobStorageService;

        public ImagesController(
            DebtCollectorContext debtCollectorContext,
            AuthorizationHelper authorizationHelper,
            BlobStorageService blobStorageService
            ) 
        {
            _context = debtCollectorContext;
            _authorizationHelper = authorizationHelper;
            _blobStorageService = blobStorageService;
        }

        [HttpPost("person")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{Path.GetExtension(file.FileName)}";

            await using var fileStream = file.OpenReadStream();
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var person = await _context.Persons.FindAsync(personId);
            if (person == null) return NotFound();

            var oldImage = await _context.Images.FindAsync(person.ImageId);
            if (oldImage != null && !oldImage.Url.Contains("avatar_placeholder.png"))
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            Image image = new() { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            person.ImageId = image.Id;
            await _context.SaveChangesAsync();

            return Ok(new ImageDTO { Id = image.Id, Url = image.Url });
        }

        [HttpPost("expense/{expenseId}")]
        public async Task<IActionResult> UploadExpenseImage([FromRoute] int expenseId, [FromForm] IFormFile file)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null) return NotFound(new { message = "Expense not found." });

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId);

            if (!isPersonInGroup) return Forbid();

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{Path.GetExtension(file.FileName)}";

            await using var fileStream = file.OpenReadStream();
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var oldImage = await _context.Images.FindAsync(expense.ImageId);
            if (oldImage != null)
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            var image = new Image { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            expense.ImageId = image.Id;
            await _context.SaveChangesAsync();

            return Ok(new ImageDTO {Id = image.Id, Url = image.Url });
        }

        [HttpPost("group/{groupId}")]
        public async Task<IActionResult> UploadGroupImage([FromRoute] int orderId, [FromForm] IFormFile file)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var group = await _context.Groups.FindAsync(orderId);
            if (group == null) return NotFound(new { message = "Group not found." });

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == group.Id);

            if (!isPersonInGroup) return Forbid();

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{Path.GetExtension(file.FileName)}";

            await using var fileStream = file.OpenReadStream();
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var oldImage = await _context.Images.FindAsync(group.ImageId);
            if (oldImage != null)
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            var image = new Image { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            group.ImageId = image.Id;
            await _context.SaveChangesAsync();

            return Ok(new ImageDTO { Id = image.Id, Url = image.Url });
        }

        [HttpPost("order/{orderId}")]
        public async Task<IActionResult> UploadOrderImage([FromRoute] int orderId, [FromForm] IFormFile file)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound(new { message = "Order not found." });

            var expense = await _context.Expenses.FindAsync(order.ExpenseId);
            if (expense == null) return NotFound(new { message = "Expense not found" });

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId);

            if (!isPersonInGroup) return Forbid();

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{Path.GetExtension(file.FileName)}";

            await using var fileStream = file.OpenReadStream();
            var blobUrl = await _blobStorageService.UploadImageAsync(uniqueFileName, fileStream);

            var oldImage = await _context.Images.FindAsync(order.ImageId);
            if (oldImage != null)
            {
                await _blobStorageService.DeleteImageAsync(oldImage.Url);
                _context.Images.Remove(oldImage);
            }

            var image = new Image { Url = blobUrl };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            order.ImageId = image.Id;
            await _context.SaveChangesAsync();

            return Ok(new ImageDTO { Id = image.Id, Url = image.Url });
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int imageId, [FromQuery] string resourceType)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var image = await _context.Images.FindAsync(imageId);
            if (image == null)
            {
                return NotFound(new { message = "Image not found" });
            }

            try
            {
                await _blobStorageService.DeleteImageAsync(image.Url);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete image from storage" });
            }

            _context.Images.Remove(image);

            if (resourceType != null)
            {
                switch (resourceType.ToLower())
                {
                    case "avatar":
                        var person = await _context.Persons.FirstOrDefaultAsync(p => p.ImageId == imageId);
                        if (person != null)
                        {
                            person.ImageId = null;
                            _context.Persons.Update(person);
                        }
                        break;

                    case "expense":
                        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.ImageId == imageId);
                        if (expense != null)
                        {
                            expense.ImageId = null;
                            _context.Expenses.Update(expense);
                        }
                        break;

                    case "order":
                        var order = await _context.Orders.FirstOrDefaultAsync(o => o.ImageId == imageId);
                        if (order != null)
                        {
                            order.ImageId = null;
                            _context.Orders.Update(order);
                        }
                        break;

                    case "group":
                        var group = await _context.Groups.FirstOrDefaultAsync(g => g.ImageId == imageId);
                        if (group != null)
                        {
                            group.ImageId = null;
                            _context.Groups.Update(group);
                        }
                        break;

                    default:
                        return BadRequest(new { message = "Invalid resource type" });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Image deleted successfully" });
        }
    }
}
