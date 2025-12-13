using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Controllers;

[Route("halls/[action]")]
public class HallsController(CinemaPlannerDbContext context) : Controller
{

    [HttpGet("/halls")]
    [HttpGet("/halls/index")]
    public async Task<IActionResult> Index()
    {
        var halls = await context.Halls.AsNoTracking().ToListAsync();
        return View(halls);
    }

    [HttpGet("/halls/details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var hall = await context.Halls.AsNoTracking()
            .Include(h => h.Screenings)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (hall == null) return NotFound();

        return View(hall);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Rows,SeatsPerRow")] Hall hall)
    {
        if (!ModelState.IsValid) return View(hall);

        context.Add(hall);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/halls/edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var hall = await context.Halls.FindAsync(id);
        if (hall == null) return NotFound();
        return View(hall);
    }

    [HttpPost("/halls/edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Rows,SeatsPerRow")] Hall hall)
    {
        if (id != hall.Id) return NotFound();
        if (!ModelState.IsValid) return View(hall);

        context.Update(hall);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/halls/delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var hall = await context.Halls.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (hall == null) return NotFound();

        return View(hall);
    }

    [HttpPost("/halls/delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var hall = await context.Halls.FindAsync(id);
        if (hall != null)
        {
            context.Halls.Remove(hall);
            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
