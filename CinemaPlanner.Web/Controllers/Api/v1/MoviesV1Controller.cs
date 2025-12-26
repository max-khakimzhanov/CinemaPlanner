using Asp.Versioning;
using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaPlanner.Web.Controllers.Api.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MoviesV1Controller(IMovieService movieService, IScreeningService screeningService) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieApiDto>>> GetAll()
    {
        var movies = await movieService.GetAllAsync();
        return Ok(movies.Select(m => new MovieApiDto(m.Id, m.Title, m.DurationMinutes, m.ReleaseYear)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieApiDto>> GetById(int id)
    {
        var movie = await movieService.GetByIdAsync(id);
        if (movie == null) return NotFound();
        return Ok(new MovieApiDto(movie.Id, movie.Title, movie.DurationMinutes, movie.ReleaseYear));
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<MovieApiDto>>> Filter([FromQuery] string? title, [FromQuery] int? minYear, [FromQuery] int? maxYear)
    {
        var movies = await movieService.GetAllAsync();
        var query = movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(m => m.Title.Contains(title));
        }
        if (minYear.HasValue)
        {
            query = query.Where(m => m.ReleaseYear >= minYear.Value);
        }
        if (maxYear.HasValue)
        {
            query = query.Where(m => m.ReleaseYear <= maxYear.Value);
        }

        return Ok(query.Select(m => new MovieApiDto(m.Id, m.Title, m.DurationMinutes, m.ReleaseYear)).ToList());
    }

    [HttpGet("{id:int}/screenings")]
    public async Task<ActionResult<IEnumerable<ScreeningApiDto>>> GetScreenings(int id)
    {
        var screenings = await screeningService.GetByMovieIdAsync(id);
        if (screenings.Count == 0) return NotFound();
        return Ok(screenings.Select(s => new ScreeningApiDto(s.Id, s.StartTime, s.HallName)));
    }

    [HttpGet("by-year/{year=2025}")]
    public async Task<ActionResult<IEnumerable<MovieApiDto>>> ByYear(int year)
    {
        var movies = await movieService.GetAllAsync();
        var query = movies.AsQueryable();
        if (year > 0)
        {
            query = query.Where(m => m.ReleaseYear == year);
        }
        else
        {
            query = query.OrderByDescending(m => m.ReleaseYear).Take(10);
        }
        return Ok(query.Select(m => new MovieApiDto(m.Id, m.Title, m.DurationMinutes, m.ReleaseYear)).ToList());
    }
}
