using CinemaPlanner.Web.Helpers;
using Xunit;

namespace CinemaPlanner.Web.Tests.Helpers;

[Trait("Area", "UI")]
public class UiBadgeHelperTests
{
    [Theory(DisplayName = "Given occupancy values, When building label, Then it formats level and percent")]
    [InlineData(0f, "Low", "Low (0.0%)")]
    [InlineData(0.5f, "Moderate", "Moderate (50.0%)")]
    [InlineData(1f, "High", "High (100.0%)")]
    public void GivenOccupancy_WhenBuildLabel_ThenFormats(float value, string level, string expected)
    {
        var label = UiBadgeHelper.BuildOccupancyLabel(value, level);

        Assert.Equal(expected, label);
        Assert.Contains("%", label);
    }

    [Theory(DisplayName = "Given out of range values, When building label, Then it clamps percent")]
    [InlineData(-1f, "Low", "Low (0.0%)")]
    [InlineData(2f, "High", "High (100.0%)")]
    public void GivenOutOfRange_WhenBuildLabel_ThenClamps(float value, string level, string expected)
    {
        var label = UiBadgeHelper.BuildOccupancyLabel(value, level);

        Assert.Equal(expected, label);
    }

    [Fact(DisplayName = "Given empty level, When building label, Then it uses fallback")]
    public void GivenEmptyLevel_WhenBuildLabel_ThenUsesFallback()
    {
        var label = UiBadgeHelper.BuildOccupancyLabel(0.25f, "");

        Assert.StartsWith("Unknown", label);
    }
}
