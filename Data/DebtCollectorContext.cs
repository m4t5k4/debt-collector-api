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

        public DbSet<Expense> Expenses { get; set; } = default!;
        public DbSet<Group> Groups { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<Payer> Payers { get; set; } = default!;
        public DbSet<Person> Persons { get; set; } = default!;
        public DbSet<PersonGroup> PersonGroups { get; set; } = default!;
        public DbSet<Debtor> Debtors { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
        public DbSet<Image> Images { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PersonGroup
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

            // Order > Expenses
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Expense)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Debtor & Payer
            modelBuilder.Entity<Payer>()
                .HasOne(pb => pb.Person)
                .WithMany()
                .HasForeignKey(pb => pb.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payer>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payers)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Debtor>()
                .HasOne(s => s.Person)
                .WithMany()
                .HasForeignKey(s => s.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Debtor>()
                .HasOne(s => s.Order)
                .WithMany(o => o.Debtors)
                .HasForeignKey(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Image
            modelBuilder.Entity<Person>()
                .HasOne(p => p.Image)
                .WithMany()
                .HasForeignKey(p => p.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Image)
                .WithMany()
                .HasForeignKey(o => o.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Image)
                .WithMany()
                .HasForeignKey(e => e.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Image)
                .WithMany()
                .HasForeignKey(g => g.ImageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
