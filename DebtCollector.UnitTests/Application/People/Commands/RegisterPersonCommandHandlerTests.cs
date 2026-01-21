using DebtCollector.Application.People.Commands.RegisterPerson;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using DebtCollector.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DebtCollector.UnitTests.Application.People.Commands
{
    public class RegisterPersonCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidRegistration_CreatesPersonWithHashedPassword()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();

            var dummyAvatar = new Image { Id = 1, Url = "https://dummy-avatar.com/default.jpg" };
            context.Images.Add(dummyAvatar);
            await context.SaveChangesAsync();

            var handler = new RegisterPersonCommandHandler(context);
            var command = new RegisterPersonCommand
            {
                Email = "newuser@example.com",
                Password = "password123",
                Username = "newuser"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("newuser@example.com");
            result.Username.Should().Be("newuser");

            var savedPerson = await context.Persons.FirstOrDefaultAsync(p => p.Email == "newuser@example.com");
            savedPerson.Should().NotBeNull();
            savedPerson!.Password.Should().NotBe("password123"); // Should be hashed
            BCrypt.Net.BCrypt.Verify("password123", savedPerson.Password).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_DuplicateEmail_ThrowsArgumentException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();

            var existingPerson = new Person
            {
                Email = "existing@example.com",
                Password = "hash",
                Username = "existing"
            };
            context.Persons.Add(existingPerson);
            await context.SaveChangesAsync();

            var handler = new RegisterPersonCommandHandler(context);
            var command = new RegisterPersonCommand
            {
                Email = "existing@example.com",
                Password = "password123",
                Username = "newuser"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Email is already taken.");
        }
    }
}
