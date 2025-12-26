using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaPlanner.Web.Controllers;

[Route("halls/[action]")]
public class HallsController(IHallService hallService) : Controller
{

    [HttpGet("/halls")]
    [HttpGet("/halls/index")]
    public async Task<IActionResult> Index()
    {
        var halls = await hallService.GetAllAsync();
        return View(halls);
    }

    [HttpGet("/halls/details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var hall = await hallService.GetByIdAsync(id.Value);
        if (hall == null) return NotFound();
        return View(hall);
    }

    [HttpGet]
    public IActionResult Create() => View(new HallCreateDto(string.Empty, 0, 0));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Rows,SeatsPerRow")] HallCreateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        await hallService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/halls/edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var hall = await hallService.GetByIdAsync(id.Value);
        if (hall == null) return NotFound();
        return View(hall);
    }

    [HttpPost("/halls/edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Rows,SeatsPerRow")] HallEditDto dto)
    {
        if (id != dto.Id) return NotFound();
        if (!ModelState.IsValid) return View(dto);
        await hallService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/halls/delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var hall = await hallService.GetByIdAsync(id.Value);
        if (hall == null) return NotFound();

        return View(hall);
    }

    [HttpPost("/halls/delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await hallService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}
