using FinEdge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinEdge.Infrastructure.Data;

public class FinEdgeDbContext(DbContextOptions<FinEdgeDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinEdgeDbContext).Assembly);

        var userBuilder = modelBuilder.Entity<User>();
        userBuilder.HasIndex(u => u.Name).IsUnique();
        userBuilder.HasIndex(u => u.Email).IsUnique();

        var walletBuilder = modelBuilder.Entity<Wallet>();
        walletBuilder.Property(w => w.Balance).HasColumnType("decimal(18,2)");

        var transactionBuilder = modelBuilder.Entity<Transaction>();
        transactionBuilder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
        transactionBuilder.Property(t => t.Type).HasConversion<string>();
        transactionBuilder.Property(t => t.Status).HasConversion<string>();
    }
}
