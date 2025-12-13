namespace CinemaPlanner.Web.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int? ReleaseYear { get; set; }
    public string? PosterUrl { get; set; }
    public int? AgeRestriction { get; set; }

    public ICollection<Screening> Screenings { get; set; } = [];
}
