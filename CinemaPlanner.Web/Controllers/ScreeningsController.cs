using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaPlanner.Web.Controllers;

[Route("screenings/[action]")]
public class ScreeningsController(IScreeningService screeningService, IMovieService movieService, IHallService hallService) : Controller
{

    [HttpGet("/screenings")]
    [HttpGet("/screenings/index")]
    public async Task<IActionResult> Index()
    {
        var screenings = await screeningService.GetAllAsync();

        return View(screenings);
    }

    [HttpGet("/screenings/upcoming/{hours:int?}")]
    public async Task<IActionResult> Upcoming(int hours = 24)
    {
        var screenings = await screeningService.GetUpcomingAsync(hours);
        ViewData["HoursWindow"] = hours;
        return View("Upcoming", screenings);
    }

    [HttpGet("/screenings/details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vm = await screeningService.GetDetailsAsync(id.Value);
        if (vm == null) return NotFound();
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
    public async Task<IActionResult> Create([Bind("MovieId,HallId,StartTime")] ScreeningCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(dto);
        }

        await screeningService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/screenings/edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var screening = await screeningService.GetForEditAsync(id.Value);
        if (screening == null) return NotFound();

        await PopulateDropdowns(screening.MovieId, screening.HallId);
        return View(screening);
    }

    [HttpPost("/screenings/edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,HallId,StartTime")] ScreeningEditDto dto)
    {
        if (id != dto.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(dto.MovieId, dto.HallId);
            return View(dto);
        }

        await screeningService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/screenings/delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var screening = await screeningService.GetDetailsAsync(id.Value);
        if (screening == null) return NotFound();
        return View(screening);
    }

    [HttpPost("/screenings/delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await screeningService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns(int? movieId = null, int? hallId = null)
    {
        var movies = await movieService.GetOptionsAsync();
        var halls = await hallService.GetOptionsAsync();

        ViewBag.MovieId = new SelectList(movies, "Id", "Title", movieId);
        ViewBag.HallId = new SelectList(halls, "Id", "Name", hallId);
    }
}
