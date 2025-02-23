using InventoryServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryServer;

public class DatabaseContext : DbContext
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<LocationCategory> LocationCategories {  get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocationCategory>()
            .HasMany(e => e.Locations)
            .WithOne(e => e.Category)
            .HasForeignKey(e => e.CategoryId)
            .HasPrincipalKey(e => e.Id);
    }
}
