using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Tests.TestData;

namespace CinemaPlanner.Web.Tests.Fixtures;

public class BookingFixture : IDisposable
{
    public string TempDirectory { get; }

    public BookingFixture()
    {
        TempDirectory = Path.Combine(Path.GetTempPath(), "CinemaPlanner.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(TempDirectory);
    }

    public Booking CreateDefault()
    {
        var booking = TestDataGenerator.Booking();
        booking.StatusFlags = 0;
        return booking;
    }

    public Booking CreateWithName(string name)
    {
        var booking = TestDataGenerator.Booking();
        booking.CustomerName = name;
        return booking;
    }

    public string CreateTempReceiptPath(int bookingId)
    {
        return Path.Combine(TempDirectory, $"receipt-{bookingId}.txt");
    }

    public void Dispose()
    {
        if (Directory.Exists(TempDirectory))
        {
            Directory.Delete(TempDirectory, true);
        }
    }
}
