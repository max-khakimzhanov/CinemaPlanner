using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Services;
using CinemaPlanner.Web.Tests.TestData;
using CinemaPlanner.Web.Tests.TestHelpers;
using CinemaPlanner.Web.Tests.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CinemaPlanner.Web.Tests.Services;

[Collection("SharedTestContext")]
[Trait("Area", "Booking")]
public class BookingServiceTests : IClassFixture<BookingFixture>
{
    private readonly BookingFixture _fixture;
    private readonly BookingService _service = new(NullLogger<BookingService>.Instance);

    public BookingServiceTests(BookingFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Given messy customer name, When booking is created, Then name is normalized for receipts")]
    public void GivenMessyName_WhenCreated_ThenNormalizeCustomerName()
    {
        var booking = _fixture.CreateWithName("   aLiCe   joHNsOn   ");

        _service.CreateBooking(booking);

        Assert.Equal("Alice Johnson", booking.CustomerName);
        BookingAssertions.AssertNormalizedName(booking.CustomerName);
    }

    [Theory(DisplayName = "Given dummy names, When booking is created, Then normalized name matches expected output")]
    [MemberData(nameof(BookingDummyData.NormalizationCases), MemberType = typeof(BookingDummyData))]
    public void GivenDummyNames_WhenCreated_ThenMatchesExpected(string input, string expected)
    {
        var booking = _fixture.CreateWithName(input);

        _service.CreateBooking(booking);

        Assert.Equal(expected, booking.CustomerName);
    }

    [Fact(DisplayName = "Given booking with long name, When booking is created, Then name is truncated to safe length")]
    public void GivenLongName_WhenCreated_ThenTruncateCustomerName()
    {
        var booking = _fixture.CreateWithName(new string('A', 200));

        _service.CreateBooking(booking);

        Assert.True(booking.CustomerName.Length <= 80);
        Assert.StartsWith("A", booking.CustomerName);
    }

    [Theory(DisplayName = "Given invalid seats, When booking is created, Then debug validation throws")]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 5)]
    public void GivenInvalidSeats_WhenCreated_ThenThrows(int row, int seat)
    {
        var booking = _fixture.CreateDefault();
        booking.SeatRow = row;
        booking.SeatNumber = seat;

#if DEBUG
        Assert.ThrowsAny<ArgumentOutOfRangeException>(() => _service.CreateBooking(booking));
#else
        _service.CreateBooking(booking);
#endif
    }

    [Fact(DisplayName = "Given a booking, When created, Then BookingCreated event is raised")]
    public void GivenBooking_WhenCreated_ThenEventRaised()
    {
        var booking = _fixture.CreateDefault();
        var raised = false;
        Booking? observed = null;

        _service.BookingCreated += (_, e) =>
        {
            raised = true;
            observed = e.Booking;
            Assert.Same(booking, e.Booking);
        };

        _service.CreateBooking(booking);

        Assert.True(raised);
        Assert.NotNull(observed);
    }

    [Theory(DisplayName = "Given generated names, When booking is created, Then name is normalized")]
    [MemberData(nameof(GeneratedNames))]
    public void GivenGeneratedNames_WhenCreated_ThenNormalized(string name)
    {
        var booking = _fixture.CreateWithName(name);

        _service.CreateBooking(booking);

        Assert.False(string.IsNullOrWhiteSpace(booking.CustomerName));
        Assert.Contains(' ', booking.CustomerName);
    }

    [Fact(Skip = "Demo-only: enable if you want to validate trimming rules visually.")]
    public void DemoOnly_Skipped_Test()
    {
        var booking = _fixture.CreateWithName(" demo ");
        _service.CreateBooking(booking);
        Assert.Equal("Demo", booking.CustomerName);
    }

    [Fact(DisplayName = "Given a booking, When created, Then it appears in service list")]
    public void GivenBooking_WhenCreated_ThenPresentInList()
    {
        var booking = _fixture.CreateDefault();

        _service.CreateBooking(booking);

        var list = _service.GetAll().ToList();
        Assert.Single(list);
        Assert.Contains(booking, list);
    }

    [Fact(DisplayName = "Given fixture temp storage, When creating receipt file, Then it is written under temp directory")]
    public void GivenFixtureTempStorage_WhenWriteReceipt_ThenExists()
    {
        var path = _fixture.CreateTempReceiptPath(42);
        File.WriteAllText(path, "dummy");

        Assert.True(File.Exists(path));
        var root = Path.GetFullPath(_fixture.TempDirectory);
        var file = Path.GetFullPath(path);
        Assert.StartsWith(root, file, StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<object[]> GeneratedNames()
    {
        yield return [TestDataGenerator.CustomerName()];
        yield return ["  john doe  "];
        yield return ["MARIA  ivanovA"];
    }
}
