using DebtCollector.Application.Images.Commands.UploadAvatar;
using DebtCollector.Application.Images.Commands.UploadExpenseImage;
using DebtCollector.Application.Images.Commands.UploadGroupImage;
using DebtCollector.Application.Images.Commands.UploadOrderImage;
using DebtCollector.Application.Images.Commands.DeleteImage;
using MediatR;
using DebtCollector.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DebtCollector.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ISender _sender;

        public ImagesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("person")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var result = await _sender.Send(new UploadAvatarCommand 
                { 
                    FileBytes = ms.ToArray(), 
                    FileName = file.FileName, 
                    ContentType = file.ContentType 
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("expense/{expenseId}")]
        public async Task<IActionResult> UploadExpenseImage([FromRoute] int expenseId, [FromForm] IFormFile file)
        {
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var result = await _sender.Send(new UploadExpenseImageCommand 
                { 
                    ExpenseId = expenseId, 
                    FileBytes = ms.ToArray(), 
                    FileName = file.FileName, 
                    ContentType = file.ContentType 
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("group/{groupId}")]
        public async Task<IActionResult> UploadGroupImage([FromRoute] int groupId, [FromForm] IFormFile file)
        {
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var result = await _sender.Send(new UploadGroupImageCommand 
                { 
                    GroupId = groupId, 
                    FileBytes = ms.ToArray(), 
                    FileName = file.FileName, 
                    ContentType = file.ContentType 
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("order/{orderId}")]
        public async Task<IActionResult> UploadOrderImage([FromRoute] int orderId, [FromForm] IFormFile file)
        {
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var result = await _sender.Send(new UploadOrderImageCommand 
                { 
                    OrderId = orderId, 
                    FileBytes = ms.ToArray(), 
                    FileName = file.FileName, 
                    ContentType = file.ContentType 
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int imageId, [FromQuery] string resourceType)
        {
            try
            {
                await _sender.Send(new DeleteImageCommand { ImageId = imageId, ResourceType = resourceType });
                return Ok(new { message = "Image deleted successfully" });
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException) { return StatusCode(500, new { message = "Failed to delete image from storage" }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}
