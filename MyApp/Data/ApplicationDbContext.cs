using Microsoft.EntityFrameworkCore;

using MyApp.Models;


namespace MyApp.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Dinner> Dinner { get; set; }
    public DbSet<Food> Food { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dinner>()
            .HasMany(e => e.Foods);

    }


}
