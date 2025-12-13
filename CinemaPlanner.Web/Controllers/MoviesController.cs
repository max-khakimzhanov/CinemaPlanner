using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Controllers;

[Route("movies/[action]")]
public class MoviesController(CinemaPlannerDbContext context, IPosterStorage posterStorage) : Controller
{

    [HttpGet("/movies")]
    [HttpGet("/movies/index")]
    public async Task<IActionResult> Index()
    {

        ViewBag.Greeting = "Welcome to the CinemaPlanner catalog";
        ViewData["Count"] = await context.Movies.CountAsync();

        var movies = await context.Movies.AsNoTracking().ToListAsync();
        foreach (var movie in movies)
        {
            movie.PosterUrl = NormalizePosterUrl(movie.PosterUrl);
        }
        return View(movies);
    }

    [HttpGet("/movies/details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return NotFound();

        movie.PosterUrl = NormalizePosterUrl(movie.PosterUrl);
        return View(movie);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,DurationMinutes,ReleaseYear")] Movie movie, IFormFile? posterFile)
    {
        if (!ModelState.IsValid) return View(movie);

        context.Add(movie);
        await context.SaveChangesAsync();

        if (posterFile != null && posterFile.Length > 0)
        {
            var objectName = $"posters/movie-{movie.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}-{posterFile.FileName}";
            movie.PosterUrl ??= await posterStorage.UploadPosterAsync(posterFile, objectName);
            context.Update(movie);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/movies/edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var movie = await context.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        movie.PosterUrl = NormalizePosterUrl(movie.PosterUrl);
        return View(movie);
    }

    [HttpPost("/movies/edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DurationMinutes,ReleaseYear,PosterUrl")] Movie movie, IFormFile? posterFile)
    {
        if (id != movie.Id) return NotFound();
        if (!ModelState.IsValid) return View(movie);

        try
        {
            if (posterFile != null && posterFile.Length > 0)
            {
                var objectName = $"posters/movie-{movie.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}-{posterFile.FileName}";
                movie.PosterUrl = await posterStorage.UploadPosterAsync(posterFile, objectName);
            }

            context.Update(movie);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await context.Movies.AnyAsync(e => e.Id == movie.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/movies/delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return NotFound();

        return View(movie);
    }

    [HttpPost("/movies/delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await context.Movies.FindAsync(id);
        if (movie != null)
        {
            context.Movies.Remove(movie);
            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/movies/poster/{id}")]
    public async Task<IActionResult> Poster(int id, CancellationToken cancellationToken)
    {
        var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (movie == null || string.IsNullOrWhiteSpace(movie.PosterUrl))
        {
            return NotFound();
        }

        var objectName = ExtractObjectName(movie.PosterUrl);
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return NotFound();
        }

        var (content, contentType) = await posterStorage.DownloadAsync(objectName, cancellationToken);
        return File(content, contentType);
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
        var movie = await context.Movies.FindAsync(value);
        return movie == null ? NotFound() : View("Details", movie);
    }

    [HttpGet("/movies/lookup/{value:alpha}")]
    public async Task<IActionResult> Lookup(string value)
    {
        var movie = await context.Movies.FirstOrDefaultAsync(m => m.Title == value);
        return movie == null ? NotFound() : View("Details", movie);
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
