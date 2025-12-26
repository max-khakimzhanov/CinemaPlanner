using CinemaPlanner.Web.Models;

using System.Globalization;

namespace CinemaPlanner.Web.Services;

public class BookingEventArgs(Booking booking) : EventArgs
{
    public Booking Booking { get; } = booking;
}

public partial class BookingService(ILogger<BookingService> logger)
{
    private readonly ILogger<BookingService> _logger = logger;
    public event EventHandler<BookingEventArgs>? BookingCreated;

    private readonly List<Booking> _bookings = [];

    public IEnumerable<Booking> GetAll() => _bookings;

    public void CreateBooking(Booking booking)
    {
        NormalizeBooking(booking);
        _bookings.Add(booking);
        OnBookingCreated(new BookingEventArgs(booking));
    }

    protected virtual void OnBookingCreated(BookingEventArgs e)
    {
        BookingCreated?.Invoke(this, e);
    }

    private static void NormalizeBooking(Booking booking)
    {
        var normalized = booking.CustomerName.Trim();
        if (normalized.Length > 80)
        {
            normalized = normalized[..80];
        }

        booking.CustomerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
            normalized.ToLower(CultureInfo.CurrentCulture));
    }
}
