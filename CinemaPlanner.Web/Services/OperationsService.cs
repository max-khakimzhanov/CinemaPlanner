using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Delegates;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class OperationsService(CinemaPlannerDbContext context) : IOperationsService
{
    public async Task<OperationsDashboardViewModel> BuildDashboardAsync()
    {
        var earliestScreening = await context.Screenings
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .OrderBy(s => s.StartTime)
            .FirstOrDefaultAsync();

        var bookingsForEarliest = earliestScreening == null
            ? []
            : await context.Bookings
                .Where(b => b.ScreeningId == earliestScreening.Id)
                .OrderBy(b => b.Id)
                .ToListAsync();

        var waitingListQueue = new Queue<string>(
            bookingsForEarliest.Count != 0
                ? bookingsForEarliest.Select(b => b.CustomerName)
                : []);
        var nextInLine = waitingListQueue.Count > 0 ? waitingListQueue.Peek() : null;

        var undoStack = new Stack<string>();
        if (earliestScreening != null)
        {
            undoStack.Push($"Scheduled {earliestScreening.Movie?.Title} @ {earliestScreening.StartTime:g}");
        }
        var lastUndo = undoStack.Count > 0 ? undoStack.Pop() : null;

        var playlist = new LinkedList<string>();
        Mapper<Models.Movie, string> map = m => $"Trailer: {m.Title}";
        await foreach (var movie in context.Movies.AsNoTracking().OrderBy(m => m.Title).AsAsyncEnumerable())
        {
            playlist.AddLast(map(movie));
        }
        if (playlist.First is not null)
        {
            playlist.AddAfter(playlist.First, "House rules & safety");
        }

        return new OperationsDashboardViewModel
        {
            WaitingList = [.. waitingListQueue],
            NextInLine = nextInLine,
            UndoStack = [.. undoStack],
            LastUndo = lastUndo,
            TrailerPlaylist = [.. playlist],
            HighlightedMovie = earliestScreening?.Movie?.Title
        };
    }
}
