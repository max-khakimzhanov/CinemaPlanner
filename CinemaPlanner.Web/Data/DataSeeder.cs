using CinemaPlanner.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CinemaPlannerDbContext>();

        await context.Database.MigrateAsync(cancellationToken);

        if (!context.Movies.Any())
        {
            context.Movies.AddRange(
                new Movie { Title = "Batman", DurationMinutes = 130, ReleaseYear = 2023 },
                new Movie { Title = "Batman 2", DurationMinutes = 128, ReleaseYear = 2024 },
                new Movie { Title = "Batman 3", DurationMinutes = 125, ReleaseYear = 2025 }
            );
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!context.Halls.Any())
        {
            context.Halls.AddRange(
                new Hall { Name = "Red", Rows = 8, SeatsPerRow = 12 },
                new Hall { Name = "Blue", Rows = 10, SeatsPerRow = 10 }
            );
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!context.Screenings.Any())
        {
            var firstMovieId = context.Movies.OrderBy(m => m.Id).Select(m => m.Id).First();
            var secondMovieId = context.Movies.OrderBy(m => m.Id).Skip(1).Select(m => m.Id).First();
            var firstHallId = context.Halls.OrderBy(h => h.Id).Select(h => h.Id).First();
            var secondHallId = context.Halls.OrderBy(h => h.Id).Skip(1).Select(h => h.Id).First();

            context.Screenings.AddRange(
                new Screening
                {
                    MovieId = firstMovieId,
                    HallId = firstHallId,
                    StartTime = DateTime.UtcNow.AddHours(2)
                },
                new Screening
                {
                    MovieId = secondMovieId,
                    HallId = secondHallId,
                    StartTime = DateTime.UtcNow.AddHours(4)
                }
            );
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!context.Bookings.Any())
        {
            var targetScreening = context.Screenings.First();
            context.Bookings.AddRange(
                new Booking { ScreeningId = targetScreening.Id, SeatRow = 1, SeatNumber = 1, CustomerName = "Alice" },
                new Booking { ScreeningId = targetScreening.Id, SeatRow = 1, SeatNumber = 2, CustomerName = "Bob" },
                new Booking { ScreeningId = targetScreening.Id, SeatRow = 2, SeatNumber = 5, CustomerName = "Charlie" }
            );
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
