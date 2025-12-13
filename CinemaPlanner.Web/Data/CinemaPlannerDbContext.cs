using CinemaPlanner.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Data;

public class CinemaPlannerDbContext(DbContextOptions<CinemaPlannerDbContext> options) : DbContext(options)
{

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<Screening> Screenings => Set<Screening>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
            entity.Property(m => m.DurationMinutes).IsRequired();
            entity.Property(m => m.PosterUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Rows).IsRequired();
            entity.Property(h => h.SeatsPerRow).IsRequired();
        });

        modelBuilder.Entity<Screening>(entity =>
        {
            entity.Property(s => s.StartTime).IsRequired();
            entity.HasOne(s => s.Movie)
                .WithMany(m => m.Screenings)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Hall)
                .WithMany(h => h.Screenings)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(b => b.CustomerName).IsRequired().HasMaxLength(200);
        });

        base.OnModelCreating(modelBuilder);
    }
}
