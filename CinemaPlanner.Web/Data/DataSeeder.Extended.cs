using CinemaPlanner.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Data;

public static partial class DataSeeder
{
    private static partial async Task SeedExtendedDataAsync(IServiceProvider services, CinemaPlannerDbContext context, CancellationToken cancellationToken)
    {
        var env = services.GetRequiredService<IHostEnvironment>();
        if (!env.IsDevelopment())
        {
            return;
        }

        var upcomingExists = await context.Screenings
            .AsNoTracking()
            .AnyAsync(s => s.StartTime >= DateTime.UtcNow.AddHours(6), cancellationToken);
        if (upcomingExists)
        {
            return;
        }

        var movies = await context.Movies.OrderBy(m => m.Id).Take(2).ToListAsync(cancellationToken);
        var halls = await context.Halls.OrderBy(h => h.Id).ToListAsync(cancellationToken);
        if (movies.Count == 0 || halls.Count == 0)
        {
            return;
        }

        var eveningStart = DateTime.UtcNow.Date.AddHours(18);
        var secondHallId = halls.Count > 1 ? halls[1].Id : halls[0].Id;

        context.Screenings.AddRange(
            new Screening
            {
                MovieId = movies[0].Id,
                HallId = halls[0].Id,
                StartTime = eveningStart
            },
            new Screening
            {
                MovieId = movies[0].Id,
                HallId = secondHallId,
                StartTime = eveningStart.AddHours(2)
            },
            new Screening
            {
                MovieId = movies[1].Id,
                HallId = halls[0].Id,
                StartTime = eveningStart.AddDays(1)
            }
        );

        await context.SaveChangesAsync(cancellationToken);
    }
}
