using CinemaPlanner.Web.Dtos;

namespace CinemaPlanner.Web.Services;

public interface IScreeningService
{
    Task<IReadOnlyList<ScreeningListDto>> GetAllAsync();
    Task<IReadOnlyList<ScreeningListDto>> GetUpcomingAsync(int hours);
    Task<IReadOnlyList<ScreeningListDto>> GetByMovieIdAsync(int movieId);
    Task<ScreeningDetailsDto?> GetDetailsAsync(int id);
    Task<ScreeningEditDto?> GetForEditAsync(int id);
    Task<int> CreateAsync(ScreeningCreateDto dto);
    Task<bool> UpdateAsync(ScreeningEditDto dto);
    Task<bool> DeleteAsync(int id);
}
