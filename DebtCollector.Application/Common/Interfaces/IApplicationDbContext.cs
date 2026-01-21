using Microsoft.EntityFrameworkCore;
using DebtCollector.Domain.Entities;

namespace DebtCollector.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Group> Groups { get; }
        DbSet<Person> Persons { get; }
        DbSet<Expense> Expenses { get; }
        DbSet<Order> Orders { get; }
        DbSet<Image> Images { get; }
        DbSet<Payer> Payers { get; }
        DbSet<Debtor> Debtors { get; }
        DbSet<PersonGroup> PersonGroups { get; }
        DbSet<RefreshToken> RefreshTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
