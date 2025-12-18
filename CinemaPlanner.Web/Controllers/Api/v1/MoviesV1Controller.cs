using Asp.Versioning;
using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Controllers.Api.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MoviesV1Controller(CinemaPlannerDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetAll()
    {
        var movies = await context.Movies.AsNoTracking().ToListAsync();
        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Movie>> GetById(int id)
    {
        var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return NotFound();
        return Ok(movie);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Movie>>> Filter([FromQuery] string? title, [FromQuery] int? minYear, [FromQuery] int? maxYear)
    {
        var movies = context.Movies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
        {
            movies = movies.Where(m => m.Title.Contains(title));
        }

        if (minYear.HasValue)
        {
            movies = movies.Where(m => m.ReleaseYear >= minYear.Value);
        }

        if (maxYear.HasValue)
        {
            movies = movies.Where(m => m.ReleaseYear <= maxYear.Value);
        }

        return Ok(await movies.ToListAsync());
    }

    [HttpGet("{id:int}/screenings")]
    public async Task<ActionResult<IEnumerable<Screening>>> GetScreenings(int id)
    {
        var exists = await context.Movies.AnyAsync(m => m.Id == id);
        if (!exists) return NotFound();

        var screenings = await context.Screenings
            .AsNoTracking()
            .Where(s => s.MovieId == id)
            .Include(s => s.Hall)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        return Ok(screenings);
    }

    [HttpGet("by-year/{year=2025}")]
    public async Task<ActionResult<IEnumerable<Movie>>> ByYear(int year)
    {
        var movies = context.Movies.AsNoTracking();

        if (year > 0)
        {
            movies = movies.Where(m => m.ReleaseYear == year);
        }
        else
        {
            movies = movies.OrderByDescending(m => m.ReleaseYear).Take(10);
        }

        return Ok(await movies.ToListAsync());
    }
}
