using Microsoft.EntityFrameworkCore;
using debt_collector_api.Models;

namespace debt_collector_api.Data
{
    public class DebtCollectorContext : DbContext
    {
        public DebtCollectorContext (DbContextOptions<DebtCollectorContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expense { get; set; } = default!;
        public DbSet<Group> Group { get; set; } = default!;
        public DbSet<Order> Order { get; set; } = default!;
        public DbSet<PaidBy> PaidBy { get; set; } = default!;
        public DbSet<Person> Person { get; set; } = default!;
        public DbSet<PersonGroup> PersonGroup { get; set; } = default!;
        public DbSet<Share> Share { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonGroup>()
                .HasKey(pg => new { pg.PersonId, pg.GroupId });

            modelBuilder.Entity<PersonGroup>()
                .HasOne(pg => pg.Person)
                .WithMany(p => p.PersonGroups)
                .HasForeignKey(pg => pg.PersonId);

            modelBuilder.Entity<PersonGroup>()
                .HasOne(pg => pg.Group)
                .WithMany(g => g.PersonGroups)
                .HasForeignKey(pg => pg.GroupId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Expense)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaidBy>()
                .HasOne(pb => pb.Person)
                .WithMany()
                .HasForeignKey(pb => pb.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Share>()
                .HasOne(s => s.Person)
                .WithMany(p => p.Shares)
                .HasForeignKey(s => s.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Share>()
                .HasOne(s => s.Order)
                .WithMany(o => o.Shares)
                .HasForeignKey(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
