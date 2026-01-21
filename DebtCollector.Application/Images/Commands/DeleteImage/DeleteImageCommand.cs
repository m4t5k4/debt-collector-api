using MediatR;

namespace DebtCollector.Application.Images.Commands.DeleteImage
{
    public class DeleteImageCommand : IRequest
    {
        public int ImageId { get; set; }
        public string? ResourceType { get; set; }
    }
}
