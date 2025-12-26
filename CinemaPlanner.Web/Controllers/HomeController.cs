using System.Diagnostics;
using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaPlanner.Web.Controllers;

public class HomeController(IHomeDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var data = await dashboardService.GetDashboardAsync();

        ViewBag.MoviesCount = data.MoviesCount;
        ViewBag.HallsCount = data.HallsCount;
        ViewBag.ScreeningsCount = data.ScreeningsCount;
        ViewBag.BookingsCount = data.BookingsCount;
        ViewBag.NextScreening = data;
        ViewBag.AverageDuration = data.AverageDuration;
        ViewBag.Occupancy = data.Occupancy;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
