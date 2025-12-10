using DebtCollector.Application.Orders.Commands.CreateOrder;
using DebtCollector.Application.DTOs;
using DebtCollector.Domain.Entities;
using DebtCollector.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DebtCollector.UnitTests.Application.Orders.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidOrder_CreatesOrderAndRecalculatesExpense()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);

            var person = new Person { Id = 1, Username = "testuser", Email = "test@example.com" };
            var group = new Group { Id = 1, Name = "Test Group", Password = "hash" };
            var expense = new Expense { Id = 1, GroupId = 1, Name = "Test Expense", TotalOrdersCost = 0 };
            
            context.Persons.Add(person);
            context.Groups.Add(group);
            context.Expenses.Add(expense);
            context.PersonGroups.Add(new PersonGroup { PersonId = 1, GroupId = 1 });
            await context.SaveChangesAsync();

            var handler = new CreateOrderCommandHandler(context, mockCurrentUserService.Object);
            var command = new CreateOrderCommand
            {
                ExpenseId = 1,
                Name = "Pizza",
                TotalCost = 25.50m,
                Payers = new List<PayerDTO> { new PayerDTO { PersonId = 1, Value = 25.50m } },
                Debtors = new List<DebtorDTO> { new DebtorDTO { PersonId = 1, Value = 25.50m } }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Pizza");
            result.TotalCost.Should().Be(25.50m);

            // Verify expense total was recalculated
            var updatedExpense = await context.Expenses.FindAsync(1);
            updatedExpense!.TotalOrdersCost.Should().Be(25.50m);
        }

        [Fact]
        public async Task Handle_WithPayersAndDebtors_CreatesRelatedEntities()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: 1);

            var person1 = new Person { Id = 1, Username = "user1", Email = "user1@example.com" };
            var person2 = new Person { Id = 2, Username = "user2", Email = "user2@example.com" };
            var group = new Group { Id = 1, Name = "Test Group", Password = "hash" };
            var expense = new Expense { Id = 1, GroupId = 1, Name = "Test Expense", TotalOrdersCost = 0 };
            
            context.Persons.AddRange(person1, person2);
            context.Groups.Add(group);
            context.Expenses.Add(expense);
            context.PersonGroups.AddRange(
                new PersonGroup { PersonId = 1, GroupId = 1 },
                new PersonGroup { PersonId = 2, GroupId = 1 }
            );
            await context.SaveChangesAsync();

            var handler = new CreateOrderCommandHandler(context, mockCurrentUserService.Object);
            var command = new CreateOrderCommand
            {
                ExpenseId = 1,
                Name = "Dinner",
                TotalCost = 50.00m,
                Payers = new List<PayerDTO> 
                { 
                    new PayerDTO { PersonId = 1, Value = 30.00m },
                    new PayerDTO { PersonId = 2, Value = 20.00m }
                },
                Debtors = new List<DebtorDTO> 
                { 
                    new DebtorDTO { PersonId = 1, Value = 25.00m },
                    new DebtorDTO { PersonId = 2, Value = 25.00m }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var payers = await context.Payers.Where(p => p.OrderId == result.Id).ToListAsync();
            payers.Should().HaveCount(2);
            payers.Sum(p => p.Value).Should().Be(50.00m);

            var debtors = await context.Debtors.Where(d => d.OrderId == result.Id).ToListAsync();
            debtors.Should().HaveCount(2);
            debtors.Sum(d => d.Value).Should().Be(50.00m);
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var mockCurrentUserService = MockServices.CreateMockCurrentUserService(personId: null);

            var handler = new CreateOrderCommandHandler(context, mockCurrentUserService.Object);
            var command = new CreateOrderCommand
            {
                ExpenseId = 1,
                Name = "Test Order",
                TotalCost = 10.00m
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
