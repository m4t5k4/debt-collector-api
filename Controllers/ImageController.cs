using debt_collector_api.Data;
using debt_collector_api.Helpers;
using debt_collector_api.Models;
using debt_collector_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly AuthorizationHelper _authorizationHelper;
        private readonly BlobStorageService _blobStorageService;

        public ImageController(
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

            return Ok(new { url = image.Url });
        }
    }
}
