using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaPlanner.Web.Controllers;

[Route("operations/[action]")]
public class OperationsController(IOperationsService operationsService) : Controller
{
    [HttpGet("/operations")]
    [HttpGet("/operations/index")]
    public async Task<IActionResult> Index()
    {
        var vm = await operationsService.BuildDashboardAsync();
        return View(vm);
    }
}
