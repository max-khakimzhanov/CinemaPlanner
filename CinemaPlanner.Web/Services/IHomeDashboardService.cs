using CinemaPlanner.Web.Dtos;

namespace CinemaPlanner.Web.Services;

public interface IHomeDashboardService
{
    Task<HomeDashboardDto> GetDashboardAsync();
}
