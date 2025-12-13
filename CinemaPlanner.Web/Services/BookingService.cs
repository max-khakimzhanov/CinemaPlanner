using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Services;

public class BookingEventArgs(Booking booking) : EventArgs
{
    public Booking Booking { get; } = booking;
}

public class BookingService
{
    public event EventHandler<BookingEventArgs>? BookingCreated;

    private readonly List<Booking> _bookings = [];

    public IEnumerable<Booking> GetAll() => _bookings;

    public void CreateBooking(Booking booking)
    {
        _bookings.Add(booking);
        OnBookingCreated(new BookingEventArgs(booking));
    }

    protected virtual void OnBookingCreated(BookingEventArgs e)
    {
        BookingCreated?.Invoke(this, e);
    }
}
