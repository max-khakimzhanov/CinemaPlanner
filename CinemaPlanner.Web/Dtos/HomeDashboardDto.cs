namespace CinemaPlanner.Web.Dtos;

public record HomeDashboardDto(
    int MoviesCount,
    int HallsCount,
    int ScreeningsCount,
    int BookingsCount,
    int? NextScreeningId,
    string? NextScreeningTitle,
    DateTime? NextScreeningTime,
    double AverageDuration,
    float Occupancy);
