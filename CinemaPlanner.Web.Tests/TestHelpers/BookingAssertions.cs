using Xunit;

namespace CinemaPlanner.Web.Tests.TestHelpers;

public static class BookingAssertions
{
    public static void AssertNormalizedName(string value)
    {
        Assert.False(string.IsNullOrWhiteSpace(value));
        Assert.Matches(@"^[A-Z][a-z]+ [A-Z][a-z]+$", value);
    }
}
