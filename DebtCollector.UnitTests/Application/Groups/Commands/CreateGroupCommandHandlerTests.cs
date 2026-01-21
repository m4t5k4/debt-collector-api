using DebtCollector.Application.Groups.Commands.CreateGroup;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using DebtCollector.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DebtCollector.UnitTests.Application.Groups.Commands
{
    public class CreateGroupCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidGroup_CreatesGroupAndReturnsDto()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);
            var mockTokenService = MockServices.CreateMockTokenService();
            var mockBlobStorageService = MockServices.CreateMockBlobStorageService();

            var person = new Person { Id = 1, Username = "testuser", Email = "test@example.com" };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var handler = new CreateGroupCommandHandler(context, mockCurrentUserService.Object, mockTokenService.Object, mockBlobStorageService.Object);
            var command = new CreateGroupCommand
            {
                Name = "Test Group"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Group");
            
            var savedGroup = await context.Groups.FirstOrDefaultAsync(g => g.Name == "Test Group");
            savedGroup.Should().NotBeNull();
            savedGroup!.Password.Should().NotBeNullOrEmpty(); // Should be generated
            
            var membership = await context.PersonGroups.FirstOrDefaultAsync(pg => pg.PersonId == 1 && pg.GroupId == savedGroup.Id);
            membership.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithImage_UploadsImageAndAssignsToGroup()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);
            var mockTokenService = MockServices.CreateMockTokenService();
            var mockBlobStorageService = MockServices.CreateMockBlobStorageService();

            var person = new Person { Id = 1, Username = "testuser", Email = "test@example.com" };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var handler = new CreateGroupCommandHandler(context, mockCurrentUserService.Object, mockTokenService.Object, mockBlobStorageService.Object);
            var command = new CreateGroupCommand
            {
                Name = "Test Group",
                Image = new byte[] { 1, 2, 3 }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockBlobStorageService.Verify(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
            
            var savedGroup = await context.Groups.Include(g => g.Image).FirstOrDefaultAsync(g => g.Id == result.Id);
            savedGroup!.Image.Should().NotBeNull();
            savedGroup.Image!.Url.Should().Be("https://mock-blob-url.com/image.jpg");
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: null);
            var mockTokenService = MockServices.CreateMockTokenService();
            var mockBlobStorageService = MockServices.CreateMockBlobStorageService();

            var handler = new CreateGroupCommandHandler(context, mockCurrentUserService.Object, mockTokenService.Object, mockBlobStorageService.Object);
            var command = new CreateGroupCommand
            {
                Name = "Test Group"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
