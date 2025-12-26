namespace CinemaPlanner.Web.Dtos;

public record MovieApiDto(
    int Id,
    string Title,
    int DurationMinutes,
    int? ReleaseYear);

public record ScreeningApiDto(
    int Id,
    DateTime StartTime,
    string HallName);
