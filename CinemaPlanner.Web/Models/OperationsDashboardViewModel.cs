namespace CinemaPlanner.Web.Models;

public class OperationsDashboardViewModel
{
    public IEnumerable<string> WaitingList { get; set; } = [];
    public string? NextInLine { get; set; }

    public IEnumerable<string> UndoStack { get; set; } = [];
    public string? LastUndo { get; set; }

    public IEnumerable<string> TrailerPlaylist { get; set; } = [];
    public string? HighlightedMovie { get; set; }
}
