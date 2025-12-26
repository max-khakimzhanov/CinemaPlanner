using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class HomeDashboardService(CinemaPlannerDbContext context) : IHomeDashboardService
{
    public async Task<HomeDashboardDto> GetDashboardAsync()
    {
        var moviesCount = await context.Movies.CountAsync();
        var hallsCount = await context.Halls.CountAsync();
        var screeningsCount = await context.Screenings.CountAsync();
        var bookingsCount = await context.Bookings.CountAsync();

        var totalSeats = 0;
        await foreach (var hall in context.Halls.AsNoTracking().AsAsyncEnumerable())
        {
            totalSeats += hall.Rows * hall.SeatsPerRow;
        }

        double averageDuration = 0;
        if (moviesCount > 0)
        {
            averageDuration = await context.Movies.AsNoTracking().AverageAsync(m => (double)m.DurationMinutes);
        }

        float occupancy = totalSeats == 0 ? 0f : bookingsCount / (float)totalSeats;

        var nextScreening = await context.Screenings
            .Include(s => s.Movie)
            .OrderBy(s => s.StartTime)
            .FirstOrDefaultAsync();

        return new HomeDashboardDto(
            moviesCount,
            hallsCount,
            screeningsCount,
            bookingsCount,
            nextScreening?.Id,
            nextScreening?.Movie?.Title,
            nextScreening?.StartTime,
            averageDuration,
            occupancy);
    }
}
