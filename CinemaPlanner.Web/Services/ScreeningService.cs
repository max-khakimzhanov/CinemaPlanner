using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Mapping;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class ScreeningService(CinemaPlannerDbContext context, SeatLayoutService seatLayoutService) : IScreeningService
{
    public async Task<IReadOnlyList<ScreeningListDto>> GetAllAsync()
    {
        var screenings = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        return screenings.Select(s => s.ToListDto()).ToList();
    }

    public async Task<IReadOnlyList<ScreeningListDto>> GetUpcomingAsync(int hours)
    {
        var from = DateTime.UtcNow;
        var to = from.AddHours(hours);
        var screenings = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Where(s => s.StartTime >= from && s.StartTime <= to)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        return screenings.Select(s => s.ToListDto()).ToList();
    }

    public async Task<IReadOnlyList<ScreeningListDto>> GetByMovieIdAsync(int movieId)
    {
        var screenings = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Where(s => s.MovieId == movieId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        return screenings.Select(s => s.ToListDto()).ToList();
    }

    public async Task<ScreeningDetailsDto?> GetDetailsAsync(int id)
    {
        var screening = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (screening == null) return null;

        var layout = seatLayoutService.BuildLayout(screening.Hall?.Rows ?? 0, screening.Hall?.SeatsPerRow ?? 0);
        return new ScreeningDetailsDto(
            screening.Id,
            screening.Movie?.Title ?? string.Empty,
            screening.Hall?.Name ?? string.Empty,
            screening.StartTime,
            screening.StartTime.ToString("g"),
            layout.SeatMatrix,
            layout.SeatLabels);
    }

    public async Task<ScreeningEditDto?> GetForEditAsync(int id)
    {
        var screening = await context.Screenings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return screening == null ? null : new ScreeningEditDto(screening.Id, screening.MovieId, screening.HallId, screening.StartTime);
    }

    public async Task<int> CreateAsync(ScreeningCreateDto dto)
    {
        var screening = new Models.Screening { MovieId = dto.MovieId, HallId = dto.HallId, StartTime = dto.StartTime };
        context.Screenings.Add(screening);
        await context.SaveChangesAsync();
        return screening.Id;
    }

    public async Task<bool> UpdateAsync(ScreeningEditDto dto)
    {
        var screening = await context.Screenings.FindAsync(dto.Id);
        if (screening == null) return false;
        screening.MovieId = dto.MovieId;
        screening.HallId = dto.HallId;
        screening.StartTime = dto.StartTime;
        context.Update(screening);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var screening = await context.Screenings.FindAsync(id);
        if (screening == null) return false;
        context.Screenings.Remove(screening);
        await context.SaveChangesAsync();
        return true;
    }

}
