using System.Diagnostics;
using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Controllers;

public class HomeController(CinemaPlannerDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var moviesCount = await context.Movies.CountAsync();
        var hallsCount = await context.Halls.CountAsync();
        var screeningsCount = await context.Screenings.CountAsync();
        var bookingsCount = await context.Bookings.CountAsync();

        var totalSeats = 0;
        await foreach (var hall in context.Halls.AsNoTracking().AsAsyncEnumerable())
        {
            totalSeats += hall.Rows * hall.SeatsPerRow;
        }

        double averageDuration = 0;
        if (moviesCount > 0)
        {
            averageDuration = await context.Movies.AsNoTracking().AverageAsync(m => (double)m.DurationMinutes);
        }

        float occupancy = totalSeats == 0 ? 0f : bookingsCount / (float)totalSeats;

        var nextScreening = await context.Screenings
            .Include(s => s.Movie)
            .OrderBy(s => s.StartTime)
            .FirstOrDefaultAsync();

        ViewBag.MoviesCount = moviesCount;
        ViewBag.HallsCount = hallsCount;
        ViewBag.ScreeningsCount = screeningsCount;
        ViewBag.BookingsCount = bookingsCount;
        ViewBag.NextScreening = nextScreening;
        ViewBag.AverageDuration = averageDuration;
        ViewBag.Occupancy = occupancy;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
