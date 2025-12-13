using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Extensions;

public static class ScreeningExtensions
{
    extension(Screening s)
    {
        public bool IsUpcoming() => s.StartTime >= DateTime.UtcNow;
        public string TimeLabel => s.StartTime.ToString("g");
    }

    extension(Screening)
    {
        public static int CountUpcoming(IEnumerable<Screening> screenings) =>
            screenings.Count(s => s.IsUpcoming());
    }
}
