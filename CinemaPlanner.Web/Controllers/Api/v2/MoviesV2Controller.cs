using Asp.Versioning;
using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaPlanner.Web.Controllers.Api.v2;

public record MovieV2Dto(int Id, string Title, int DurationMinutes, int? ReleaseYear, string DurationLabel);

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MoviesV2Controller(IMovieService movieService) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieV2Dto>>> GetAll()
    {
        var movies = await movieService.GetAllAsync();
        return Ok(movies.Select(m => new MovieV2Dto(m.Id, m.Title, m.DurationMinutes, m.ReleaseYear, m.DurationLabel)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieV2Dto>> GetById(int id)
    {
        var movie = await movieService.GetByIdAsync(id);
        if (movie == null) return NotFound();
        return Ok(new MovieV2Dto(movie.Id, movie.Title, movie.DurationMinutes, movie.ReleaseYear, movie.DurationLabel));
    }
}
