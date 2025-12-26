using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaPlanner.Web.Controllers;

[Route("movies/[action]")]
public class MoviesController(IMovieService movieService) : Controller
{

    [HttpGet("/movies")]
    [HttpGet("/movies/index")]
    public async Task<IActionResult> Index()
    {

        ViewBag.Greeting = "Welcome to the CinemaPlanner catalog";
        var movies = await movieService.GetAllAsync();
        ViewData["Count"] = movies.Count;

        return View(movies);
    }

    [HttpGet("/movies/details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var movie = await movieService.GetByIdAsync(id.Value);
        if (movie == null) return NotFound();

        return View(movie);
    }

    [HttpGet]
    public IActionResult Create() => View(new MovieCreateDto(string.Empty, 0, null));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,DurationMinutes,ReleaseYear")] MovieCreateDto dto, IFormFile? posterFile)
    {
        if (!ModelState.IsValid) return View(dto);

        await movieService.CreateAsync(dto, posterFile);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/movies/edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var movie = await movieService.GetForEditAsync(id.Value);
        if (movie == null) return NotFound();

        return View(movie);
    }

    [HttpPost("/movies/edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DurationMinutes,ReleaseYear,PosterUrl")] MovieEditDto dto, IFormFile? posterFile)
    {
        if (id != dto.Id) return NotFound();
        if (!ModelState.IsValid) return View(dto);

        var updated = await movieService.UpdateAsync(dto, posterFile);
        if (!updated) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/movies/delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var movie = await movieService.GetByIdAsync(id.Value);
        if (movie == null) return NotFound();

        return View(movie);
    }

    [HttpPost("/movies/delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await movieService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/movies/poster/{id}")]
    public async Task<IActionResult> Poster(int id, CancellationToken cancellationToken)
    {
        var result = await movieService.GetPosterAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return File(result.Value.Content, result.Value.ContentType);
    }

    private string? NormalizePosterUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;

        var scheme = "http";
        return $"{scheme}://{url.TrimStart('/')}";
    } 

    private string? ExtractObjectName(string posterUrl)
    {
        if (string.IsNullOrWhiteSpace(posterUrl)) return null;
        try
        {
            if (Uri.TryCreate(posterUrl, UriKind.Absolute, out var uri))
            {
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2)
                {
                    return string.Join('/', segments.Skip(1));
                }
            }
            else
            {
                var parts = posterUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return null;
                if (parts.Length == 1) return parts[0];
                return string.Join('/', parts.Skip(1));
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    [HttpGet("/movies/lookup/{value:int}")]
    public async Task<IActionResult> Lookup(int value)
    {
        var movie = await movieService.GetByIdAsync(value);
        return movie == null ? NotFound() : View("Details", movie);
    }

    [HttpGet("/movies/lookup/{value:alpha}")]
    public async Task<IActionResult> Lookup(string value)
    {
        var movie = await movieService.GetAllAsync();
        var match = movie.FirstOrDefault(m => m.Title == value);
        return match == null ? NotFound() : View("Details", match);
    }

    [HttpPost("/movies/lookup")]
    public IActionResult Lookup([FromForm] int id, [FromForm] string name)
    {
        if (id != 0)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        return string.IsNullOrWhiteSpace(name)
            ? BadRequest()
            : RedirectToAction(nameof(Lookup), new { value = name });
    }
}
