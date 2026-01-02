using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Services;

public sealed class LoggingBookingNotifier(ILogger<LoggingBookingNotifier> logger) : BookingNotifierBase
{
    public override string Channel => "OperationsLog";

    protected override Task SendAsync(Booking booking, string message, CancellationToken cancellationToken)
    {
        logger.LogInformation("[{Channel}] {Message}", Channel, message);
        return Task.CompletedTask;
    }
}
