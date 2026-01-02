using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Services;

public abstract class BookingNotifierBase : IBookingNotifier, INotificationChannel
{
    public abstract string Channel { get; }

    public async Task NotifyAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var message = BuildMessage(booking);
        await SendAsync(booking, message, cancellationToken);
    }

    protected virtual string BuildMessage(Booking booking)
    {
        return $"Booking {booking.Id} for screening {booking.ScreeningId} ({booking.CustomerName}).";
    }

    protected abstract Task SendAsync(Booking booking, string message, CancellationToken cancellationToken);
}
