using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Mapping;

public static class HallMapping
{
    extension(Hall h)
    {
        public HallListDto ToListDto() => new(
            h.Id,
            h.Name,
            h.Rows,
            h.SeatsPerRow,
            h.Rows * h.SeatsPerRow);

        public HallEditDto ToEditDto() => new(
            h.Id,
            h.Name,
            h.Rows,
            h.SeatsPerRow);

        public (int Id, string Name) ToOption() => (h.Id, h.Name);
    }
}
