using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Services;

public interface IBookingNotifier
{
    Task NotifyAsync(Booking booking, CancellationToken cancellationToken = default);
}
