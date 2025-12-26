namespace CinemaPlanner.Web.Dtos;

public record HallListDto(
    int Id,
    string Name,
    int Rows,
    int SeatsPerRow,
    int Capacity);

public record HallEditDto(
    int Id,
    string Name,
    int Rows,
    int SeatsPerRow);

public record HallCreateDto(
    string Name,
    int Rows,
    int SeatsPerRow);
