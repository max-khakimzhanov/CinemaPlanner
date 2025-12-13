using Asp.Versioning;
using CinemaPlanner.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Controllers.Api.v2;

public record MovieV2Dto(int Id, string Title, int DurationMinutes, int? ReleaseYear, string DurationLabel);

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/movies")]
public class MoviesV2Controller(CinemaPlannerDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieV2Dto>>> GetAll()
    {
        var movies = await context.Movies
            .AsNoTracking()
            .Select(m => new MovieV2Dto(
                m.Id,
                m.Title,
                m.DurationMinutes,
                m.ReleaseYear,
                $"{m.DurationMinutes} min"))
            .ToListAsync();

        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieV2Dto>> GetById(int id)
    {
        var dto = await context.Movies
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MovieV2Dto(
                m.Id,
                m.Title,
                m.DurationMinutes,
                m.ReleaseYear,
                $"{m.DurationMinutes} min"))
            .FirstOrDefaultAsync();

        if (dto == null) return NotFound();
        return Ok(dto);
    }
}

