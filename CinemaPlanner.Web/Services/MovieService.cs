using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Mapping;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class MovieService(CinemaPlannerDbContext context, IPosterStorage posterStorage) : IMovieService
{
    public async Task<IReadOnlyList<MovieListDto>> GetAllAsync()
    {
        var movies = await context.Movies.AsNoTracking().ToListAsync();
        return movies.Select(m => m.ToListDto()).ToList();
    }

    public async Task<MovieDetailDto?> GetByIdAsync(int id)
    {
        var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return movie == null ? null : movie.ToDetailDto();
    }

    public async Task<MovieEditDto?> GetForEditAsync(int id)
    {
        var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return movie == null ? null : movie.ToEditDto();
    }

    public async Task<int> CreateAsync(MovieCreateDto dto, IFormFile? posterFile)
    {
        var movie = new Models.Movie
        {
            Title = dto.Title,
            DurationMinutes = dto.DurationMinutes,
            ReleaseYear = dto.ReleaseYear
        };
        context.Movies.Add(movie);
        await context.SaveChangesAsync();

        if (posterFile is { Length: > 0 })
        {
            var objectName = $"posters/movie-{movie.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}-{posterFile.FileName}";
            movie.PosterUrl = await posterStorage.UploadPosterAsync(posterFile, objectName);
            context.Movies.Update(movie);
            await context.SaveChangesAsync();
        }

        return movie.Id;
    }

    public async Task<bool> UpdateAsync(MovieEditDto dto, IFormFile? posterFile)
    {
        var movie = await context.Movies.FindAsync(dto.Id);
        if (movie == null) return false;

        movie.Title = dto.Title;
        movie.DurationMinutes = dto.DurationMinutes;
        movie.ReleaseYear = dto.ReleaseYear;

        if (posterFile is { Length: > 0 })
        {
            var objectName = $"posters/movie-{movie.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}-{posterFile.FileName}";
            movie.PosterUrl = await posterStorage.UploadPosterAsync(posterFile, objectName);
        }
        else
        {
            movie.PosterUrl = dto.PosterUrl;
        }

        context.Update(movie);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await context.Movies.FindAsync(id);
        if (movie == null) return false;
        context.Movies.Remove(movie);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<(int Id, string Title)>> GetOptionsAsync()
    {
        var movies = await context.Movies.AsNoTracking().OrderBy(m => m.Title).ToListAsync();
        return movies.Select(m => m.ToOption()).ToList();
    }

    public async Task<(Stream Content, string ContentType)?> GetPosterAsync(int id, CancellationToken cancellationToken)
    {
        var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (movie == null || string.IsNullOrWhiteSpace(movie.PosterUrl)) return null;

        var objectName = ExtractObjectName(movie.PosterUrl);
        if (string.IsNullOrWhiteSpace(objectName)) return null;

        return await posterStorage.DownloadAsync(objectName, cancellationToken);
    }

    private static string? ExtractObjectName(string posterUrl)
    {
        if (string.IsNullOrWhiteSpace(posterUrl)) return null;
        if (Uri.TryCreate(posterUrl, UriKind.Absolute, out var uri))
        {
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return segments.Length >= 2 ? string.Join('/', segments.Skip(1)) : null;
        }
        var parts = posterUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return null;
        return parts.Length == 1 ? parts[0] : string.Join('/', parts.Skip(1));
    }
}
