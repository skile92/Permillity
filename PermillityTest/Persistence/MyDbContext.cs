using Microsoft.EntityFrameworkCore;

namespace PermillityTest.Persistence;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>()
            .ToTable("Weather");

        modelBuilder.Entity<WeatherForecast>()
            .HasKey(x => x.Id);

        base.OnModelCreating(modelBuilder);
    }
}
