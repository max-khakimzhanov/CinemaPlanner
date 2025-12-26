namespace CinemaPlanner.Web.Helpers;

public static class UiBadgeHelper
{
    public static string BuildOccupancyLabel(float occupancy, string? level)
    {
        var safeLevel = string.IsNullOrWhiteSpace(level) ? "Unknown" : level;
        var percent = Math.Clamp(occupancy, 0f, 1f) * 100f;
        return $"{safeLevel} ({percent:F1}%)";
    }
}
