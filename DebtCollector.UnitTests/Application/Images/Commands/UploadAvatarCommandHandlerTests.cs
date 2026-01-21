using DebtCollector.Application.Images.Commands.UploadAvatar;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using DebtCollector.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DebtCollector.UnitTests.Application.Images.Commands
{
    public class UploadAvatarCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidImage_UploadsAndReturnsDto()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);
            var mockBlobStorageService = MockServices.CreateMockBlobStorageService();

            var person = new Person { Id = 1, Username = "testuser", Email = "test@example.com" };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var handler = new UploadAvatarCommandHandler(context, mockCurrentUserService.Object, mockBlobStorageService.Object);
            var command = new UploadAvatarCommand
            {
                FileBytes = new byte[] { 1, 2, 3, 4, 5 },
                FileName = "avatar.jpg",
                ContentType = "image/jpeg"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Url.Should().Be("https://mock-blob-url.com/image.jpg");

            mockBlobStorageService.Verify(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);

            var updatedPerson = await context.Persons.Include(p => p.Image).FirstOrDefaultAsync(p => p.Id == 1);
            updatedPerson!.Image.Should().NotBeNull();
            updatedPerson.Image!.Url.Should().Be("https://mock-blob-url.com/image.jpg");
        }

        [Fact]
        public async Task Handle_InvalidFileType_ThrowsArgumentException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);
            var mockBlobStorageService = MockServices.CreateMockBlobStorageService();

            var person = new Person { Id = 1, Username = "testuser", Email = "test@example.com" };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var handler = new UploadAvatarCommandHandler(context, mockCurrentUserService.Object, mockBlobStorageService.Object);
            var command = new UploadAvatarCommand
            {
                FileBytes = new byte[] { 1, 2, 3 },
                FileName = "document.pdf",
                ContentType = "application/pdf"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Only image files (JPEG, PNG, GIF, WEBP) are allowed.");
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: null);
            var mockBlobStorageService = MockServices.CreateMockBlobStorageService();

            var handler = new UploadAvatarCommandHandler(context, mockCurrentUserService.Object, mockBlobStorageService.Object);
            var command = new UploadAvatarCommand
            {
                FileBytes = new byte[] { 1, 2, 3 },
                FileName = "avatar.jpg",
                ContentType = "image/jpeg"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
