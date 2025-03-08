using InventoryServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryServer;

public class DatabaseContext : DbContext
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<LocationCategory> LocationCategories {  get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<PartCategory> PartCategories { get; set; }
    public DbSet<StockEntry> StockEntries { get; set; }
    public DbSet<User> Users { get; set; }

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

        modelBuilder.Entity<Part>()
            .HasMany(e => e.Entries)
            .WithOne(e => e.Part)
            .HasForeignKey(e => e.PartId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<User>()
            .HasMany(e => e.StockEntries)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<Location>()
            .HasMany(e => e.Parts)
            .WithOne(e => e.Location)
            .HasForeignKey(e => e.LocationId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<PartCategory>()
            .HasMany(e => e.Parts)
            .WithOne(e => e.Category)
            .HasForeignKey(e => e.CategoryId)
            .HasPrincipalKey(e => e.Id);
    }
}
