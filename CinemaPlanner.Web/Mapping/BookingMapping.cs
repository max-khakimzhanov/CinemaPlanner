using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Mapping;

public static class BookingMapping
{
    extension(Booking b)
    {
        public BookingListDto ToListDto() => new(
            b.Id,
            b.Screening?.Movie?.Title ?? string.Empty,
            b.Screening?.StartTime ?? DateTime.MinValue,
            b.SeatRow,
            b.SeatNumber,
            b.CustomerName,
            (BookingStatusFlags)b.StatusFlags);
    }
}
