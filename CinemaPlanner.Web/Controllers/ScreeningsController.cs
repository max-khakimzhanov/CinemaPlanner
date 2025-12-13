using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Controllers;

[Route("screenings/[action]")]
public class ScreeningsController(CinemaPlannerDbContext context, SeatLayoutService seatLayoutService) : Controller
{

    [HttpGet("/screenings")]
    [HttpGet("/screenings/index")]
    public async Task<IActionResult> Index()
    {
        var screenings = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        return View(screenings);
    }

    [HttpGet("/screenings/upcoming/{hours:int?}")]
    public async Task<IActionResult> Upcoming(int hours = 24)
    {
        var from = DateTime.UtcNow;
        var to = from.AddHours(hours);
        var screenings = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Where(s => s.StartTime >= from && s.StartTime <= to)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
        ViewData["HoursWindow"] = hours;
        return View("Upcoming", screenings);
    }

    [HttpGet("/screenings/details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var screening = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (screening == null) return NotFound();

        var layout = seatLayoutService.BuildLayout(screening.Hall?.Rows ?? 0, screening.Hall?.SeatsPerRow ?? 0);
        var vm = new ScreeningDetailsViewModel
        {
            Screening = screening,
            SeatMatrix = layout.SeatMatrix,
            SeatLabels = layout.SeatLabels
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MovieId,HallId,StartTime")] Screening screening)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(screening);
        }

        context.Add(screening);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/screenings/edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var screening = await context.Screenings.FindAsync(id);
        if (screening == null) return NotFound();

        await PopulateDropdowns(screening.MovieId, screening.HallId);
        return View(screening);
    }

    [HttpPost("/screenings/edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,HallId,StartTime")] Screening screening)
    {
        if (id != screening.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(screening.MovieId, screening.HallId);
            return View(screening);
        }

        context.Update(screening);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/screenings/delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var screening = await context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (screening == null) return NotFound();

        return View(screening);
    }

    [HttpPost("/screenings/delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var screening = await context.Screenings.FindAsync(id);
        if (screening != null)
        {
            context.Screenings.Remove(screening);
            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns(int? movieId = null, int? hallId = null)
    {
        var movies = await context.Movies.AsNoTracking().OrderBy(m => m.Title).ToListAsync();
        var halls = await context.Halls.AsNoTracking().OrderBy(h => h.Name).ToListAsync();

        ViewBag.MovieId = new SelectList(movies, "Id", "Title", movieId);
        ViewBag.HallId = new SelectList(halls, "Id", "Name", hallId);
    }
}
