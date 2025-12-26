using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Mapping;

public static class ScreeningMapping
{
    extension(Screening s)
    {
        public ScreeningListDto ToListDto() => new(
            s.Id,
            s.Movie?.Title ?? string.Empty,
            s.Hall?.Name ?? string.Empty,
            s.StartTime,
            s.StartTime.ToString("g"));

        public ScreeningApiDto ToApiDto() => new(
            s.Id,
            s.StartTime,
            s.Hall?.Name ?? string.Empty);
    }
}
