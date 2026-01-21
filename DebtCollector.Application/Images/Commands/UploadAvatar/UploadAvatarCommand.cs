using DebtCollector.Application.DTOs;
using MediatR;

namespace DebtCollector.Application.Images.Commands.UploadAvatar
{
    public class UploadAvatarCommand : IRequest<ImageDTO>
    {
        public byte[] FileBytes { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
