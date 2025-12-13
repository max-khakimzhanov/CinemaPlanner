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
}
