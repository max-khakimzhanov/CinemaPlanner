namespace CinemaPlanner.Web.Dtos;

public record ScreeningListDto(
    int Id,
    string MovieTitle,
    string HallName,
    DateTime StartTime,
    string TimeLabel);

public record ScreeningEditDto(
    int Id,
    int MovieId,
    int HallId,
    DateTime StartTime);

public record ScreeningCreateDto(
    int MovieId,
    int HallId,
    DateTime StartTime);

public record ScreeningDetailsDto(
    int Id,
    string MovieTitle,
    string HallName,
    DateTime StartTime,
    string TimeLabel,
    char[,] SeatMatrix,
    string[][] SeatLabels);
