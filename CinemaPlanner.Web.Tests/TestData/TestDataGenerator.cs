using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Tests.TestData;

public static class TestDataGenerator
{
    private static readonly Random Random = new(1234);

    public static string CustomerName()
    {
        var firstNames = new[] { "alice", "bob", "charlie", "diana", "eric" };
        var lastNames = new[] { "smith", "johnson", "brown", "miller", "lee" };
        var first = firstNames[Random.Next(firstNames.Length)];
        var last = lastNames[Random.Next(lastNames.Length)];
        return $"{first} {last}";
    }

    public static Booking Booking()
    {
        return new Booking
        {
            ScreeningId = 1,
            SeatRow = 3,
            SeatNumber = 7,
            CustomerName = CustomerName()
        };
    }
}
