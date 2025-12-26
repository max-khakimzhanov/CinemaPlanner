using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Services;

public interface IOperationsService
{
    Task<OperationsDashboardViewModel> BuildDashboardAsync();
}
