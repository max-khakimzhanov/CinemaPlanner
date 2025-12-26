namespace CinemaPlanner.Web.Dtos;

public record MovieListDto(
    int Id,
    string Title,
    int DurationMinutes,
    int? ReleaseYear,
    string? PosterUrl,
    string DisplayTitle,
    string DurationLabel);

public record MovieDetailDto(
    int Id,
    string Title,
    int DurationMinutes,
    int? ReleaseYear,
    string? PosterUrl,
    string DisplayTitle,
    string DurationLabel);

public record MovieCreateDto(
    string Title,
    int DurationMinutes,
    int? ReleaseYear);

public record MovieEditDto(
    int Id,
    string Title,
    int DurationMinutes,
    int? ReleaseYear,
    string? PosterUrl);
