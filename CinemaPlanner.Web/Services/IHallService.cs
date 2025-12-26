using CinemaPlanner.Web.Dtos;

namespace CinemaPlanner.Web.Services;

public interface IHallService
{
    Task<IReadOnlyList<HallListDto>> GetAllAsync();
    Task<HallEditDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(HallCreateDto dto);
    Task<bool> UpdateAsync(HallEditDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IReadOnlyList<(int Id, string Name)>> GetOptionsAsync();
}
