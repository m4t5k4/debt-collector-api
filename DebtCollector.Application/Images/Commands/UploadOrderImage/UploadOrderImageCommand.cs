using DebtCollector.Application.DTOs;
using MediatR;

namespace DebtCollector.Application.Images.Commands.UploadOrderImage
{
    public class UploadOrderImageCommand : IRequest<ImageDTO>
    {
        public int OrderId { get; set; }
        public byte[] FileBytes { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
