using System.Numerics;
using CinemaPlanner.Web.Dtos;

namespace CinemaPlanner.Web.Services;

public interface IBookingAppService
{
    Task<IReadOnlyList<BookingListDto>> GetAllAsync();
    Task CreateAsync(BookingCreateDto dto);
    Task<IReadOnlyList<(int Id, string Label)>> GetScreeningOptionsAsync();
    Task<BigInteger?> GetSeatCombinationEstimateAsync();
    bool IsBookingDisabled { get; }
    string AdminEmail { get; }
}
