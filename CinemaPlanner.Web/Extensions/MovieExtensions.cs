using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Extensions;

public static class MovieExtensions
{
    extension(Movie m)
    {
        public string DurationLabel => $"{m.DurationMinutes} min";
        public string DisplayTitle => m.ReleaseYear is null ? m.Title : $"{m.Title} ({m.ReleaseYear})";
    }

    extension(Movie)
    {
        public static IEnumerable<string> Titles(IEnumerable<Movie> movies) =>
            movies.Select(m => m.DisplayTitle);
    }
}
