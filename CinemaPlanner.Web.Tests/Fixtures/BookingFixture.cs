using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Tests.TestData;

namespace CinemaPlanner.Web.Tests.Fixtures;

public class BookingFixture
{
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
}
