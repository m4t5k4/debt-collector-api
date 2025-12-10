using DebtCollector.Application.Auth.Commands.Login;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using DebtCollector.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DebtCollector.UnitTests.Application.Auth.Commands
{
    public class LoginCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCredentials_ReturnsAuthResult()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockTokenService = MockServices.CreateMockTokenService();

            var person = new Person
            {
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Username = "testuser",
                IsVerified = true
            };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var handler = new LoginCommandHandler(context, mockTokenService.Object);
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be("mock-jwt-token");
            result.RefreshToken.Should().Be("mock-refresh-token");
            
            // Verify refresh token was saved to database
            var savedToken = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.PersonId == person.Id);
            savedToken.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_InvalidEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockTokenService = MockServices.CreateMockTokenService();

            var handler = new LoginCommandHandler(context, mockTokenService.Object);
            var command = new LoginCommand
            {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid user");
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockTokenService = MockServices.CreateMockTokenService();

            var person = new Person
            {
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Username = "testuser"
            };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var handler = new LoginCommandHandler(context, mockTokenService.Object);
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid password");
        }
    }
}
