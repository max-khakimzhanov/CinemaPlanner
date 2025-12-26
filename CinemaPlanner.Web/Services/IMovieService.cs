using CinemaPlanner.Web.Dtos;

namespace CinemaPlanner.Web.Services;

public interface IMovieService
{
    Task<IReadOnlyList<MovieListDto>> GetAllAsync();
    Task<MovieDetailDto?> GetByIdAsync(int id);
    Task<MovieEditDto?> GetForEditAsync(int id);
    Task<int> CreateAsync(MovieCreateDto dto, IFormFile? posterFile);
    Task<bool> UpdateAsync(MovieEditDto dto, IFormFile? posterFile);
    Task<bool> DeleteAsync(int id);
    Task<(Stream Content, string ContentType)?> GetPosterAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<(int Id, string Title)>> GetOptionsAsync();
}
