using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.UnitTests.Helpers
{
    public static class TestDbContextFactory
    {
        public static DebtCollectorContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DebtCollectorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new DebtCollectorContext(options);
        }
    }
}
