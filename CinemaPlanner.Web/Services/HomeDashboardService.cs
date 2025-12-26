using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class HomeDashboardService(CinemaPlannerDbContext context, ILogger<HomeDashboardService> logger) : IHomeDashboardService
{
    public async Task<HomeDashboardDto> GetDashboardAsync()
    {
        var moviesCount = await context.Movies.CountAsync();
        var hallsCount = await context.Halls.CountAsync();
        var screeningsCount = await context.Screenings.CountAsync();
        var bookingsCount = await context.Bookings.CountAsync();

        var totalSeats = 0;
        var overflowed = false;
        await foreach (var hall in context.Halls.AsNoTracking().AsAsyncEnumerable())
        {
            try
            {
                checked
                {
                    totalSeats = checked(totalSeats + checked(hall.Rows * hall.SeatsPerRow));
                }
            }
            catch (OverflowException)
            {
                overflowed = true;
                totalSeats = int.MaxValue;
                break;
            }
        }
        if (overflowed)
        {
            logger.LogWarning("Seat count overflowed during dashboard calculation; value capped.");
        }

        double averageDuration = 0;
        if (moviesCount > 0)
        {
            averageDuration = await context.Movies.AsNoTracking().AverageAsync(m => (double)m.DurationMinutes);
        }

        float occupancy = totalSeats == 0 ? 0f : bookingsCount / (float)totalSeats;
        var occupancyLevel = occupancy >= 0.75f
            ? "High"
            : occupancy >= 0.4f ? "Moderate" : "Low";

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
            occupancy,
            occupancyLevel);
    }
}
