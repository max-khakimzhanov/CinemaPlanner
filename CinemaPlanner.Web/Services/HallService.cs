using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Mapping;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class HallService(CinemaPlannerDbContext context) : IHallService
{
    public async Task<IReadOnlyList<HallListDto>> GetAllAsync()
    {
        var halls = await context.Halls.AsNoTracking().ToListAsync();
        return halls.Select(h => h.ToListDto()).ToList();
    }

    public async Task<HallEditDto?> GetByIdAsync(int id)
    {
        var hall = await context.Halls.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
        return hall == null ? null : hall.ToEditDto();
    }

    public async Task<int> CreateAsync(HallCreateDto dto)
    {
        var hall = new Models.Hall { Name = dto.Name, Rows = dto.Rows, SeatsPerRow = dto.SeatsPerRow };
        context.Halls.Add(hall);
        await context.SaveChangesAsync();
        return hall.Id;
    }

    public async Task<bool> UpdateAsync(HallEditDto dto)
    {
        var hall = await context.Halls.FindAsync(dto.Id);
        if (hall == null) return false;
        hall.Name = dto.Name;
        hall.Rows = dto.Rows;
        hall.SeatsPerRow = dto.SeatsPerRow;
        context.Update(hall);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hall = await context.Halls.FindAsync(id);
        if (hall == null) return false;
        context.Halls.Remove(hall);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<(int Id, string Name)>> GetOptionsAsync()
    {
        var halls = await context.Halls.AsNoTracking().OrderBy(h => h.Name).ToListAsync();
        return halls.Select(h => h.ToOption()).ToList();
    }
}
