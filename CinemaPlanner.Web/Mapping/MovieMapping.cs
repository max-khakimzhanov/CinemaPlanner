using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Mapping;

public static class MovieMapping
{
    extension(Movie m)
    {
        public MovieListDto ToListDto() => new(
            m.Id,
            m.Title,
            m.DurationMinutes,
            m.ReleaseYear,
            NormalizePosterUrl(m.PosterUrl),
            m.ReleaseYear is null ? m.Title : $"{m.Title} ({m.ReleaseYear})",
            $"{m.DurationMinutes} min");

        public MovieDetailDto ToDetailDto() => new(
            m.Id,
            m.Title,
            m.DurationMinutes,
            m.ReleaseYear,
            NormalizePosterUrl(m.PosterUrl),
            m.ReleaseYear is null ? m.Title : $"{m.Title} ({m.ReleaseYear})",
            $"{m.DurationMinutes} min");

        public MovieEditDto ToEditDto() => new(
            m.Id,
            m.Title,
            m.DurationMinutes,
            m.ReleaseYear,
            m.PosterUrl);

        public MovieApiDto ToApiDto() => new(
            m.Id,
            m.Title,
            m.DurationMinutes,
            m.ReleaseYear);

        public (int Id, string Title) ToOption() => (m.Id, m.Title);
    }

    private static string? NormalizePosterUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;
        return $"http://{url.TrimStart('/')}";
    }
}
