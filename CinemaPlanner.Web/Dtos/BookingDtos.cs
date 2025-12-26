using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Dtos;

public record BookingListDto(
    int Id,
    string ScreeningTitle,
    DateTime StartTime,
    int SeatRow,
    int SeatNumber,
    string CustomerName,
    BookingStatusFlags Status);

public record BookingCreateDto(
    int ScreeningId,
    int SeatRow,
    int SeatNumber,
    string CustomerName);
