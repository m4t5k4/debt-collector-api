using DebtCollector.Application.DTOs;
using MediatR;

namespace DebtCollector.Application.Images.Commands.UploadExpenseImage
{
    public class UploadExpenseImageCommand : IRequest<ImageDTO>
    {
        public int ExpenseId { get; set; }
        public byte[] FileBytes { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
