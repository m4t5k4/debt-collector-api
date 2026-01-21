using DebtCollector.Application.Expenses.Commands.CreateExpense;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using DebtCollector.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DebtCollector.UnitTests.Application.Expenses.Commands
{
    public class CreateExpenseCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidExpense_CreatesExpenseAndReturnsDto()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);

            var person = new Person { Id = 1, Username = "testuser", Email = "test@example.com" };
            var group = new Group { Id = 1, Name = "Test Group", Password = "hash" };
            context.Persons.Add(person);
            context.Groups.Add(group);
            context.PersonGroups.Add(new PersonGroup { PersonId = 1, GroupId = 1 });
            await context.SaveChangesAsync();

            var handler = new CreateExpenseCommandHandler(context, mockCurrentUserService.Object);
            var command = new CreateExpenseCommand
            {
                GroupId = 1,
                Name = "Groceries"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Groceries");
            result.TotalOrdersCost.Should().Be(0);

            var savedExpense = await context.Expenses.FirstOrDefaultAsync(e => e.Name == "Groceries");
            savedExpense.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: null);

            var handler = new CreateExpenseCommandHandler(context, mockCurrentUserService.Object);
            var command = new CreateExpenseCommand
            {
                GroupId = 1,
                Name = "Test Expense"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_UserNotInGroup_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);

            var person = new Person { Id = 1, Username = "testuser", Email = "test@example.com" };
            var group = new Group { Id = 1, Name = "Test Group", Password = "hash" };
            context.Persons.Add(person);
            context.Groups.Add(group);
            // Note: NOT adding PersonGroup membership
            await context.SaveChangesAsync();

            var handler = new CreateExpenseCommandHandler(context, mockCurrentUserService.Object);
            var command = new CreateExpenseCommand
            {
                GroupId = 1,
                Name = "Test Expense"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("You are not a member of this group.");
        }
    }
}
