using CinemaPlanner.Web.Services;
using CinemaPlanner.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaPlanner.Web.Controllers;

[Route("booking/[action]")]
public class BookingController : Controller
{
    private readonly IBookingAppService _bookingService;

    public BookingController(IBookingAppService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("/booking")]
    [HttpGet("/booking/index")]
    public async Task<IActionResult> Index()
    {
        var bookings = await _bookingService.GetAllAsync();
        var combinations = await _bookingService.GetSeatCombinationEstimateAsync();

        ViewBag.SeatCombinationEstimate = combinations;
        ViewBag.BookingsDisabled = _bookingService.IsBookingDisabled;
        ViewBag.AdminEmail = _bookingService.AdminEmail;

        return View(bookings);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (_bookingService.IsBookingDisabled)
        {
            return Forbid("Bookings are disabled via configuration.");
        }
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ScreeningId,SeatRow,SeatNumber,CustomerName")] BookingCreateDto dto)
    {
        if (_bookingService.IsBookingDisabled)
        {
            return Forbid("Bookings are disabled via configuration.");
        }
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(dto);
        }

        await _bookingService.CreateAsync(dto);

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        var screenings = await _bookingService.GetScreeningOptionsAsync();
        ViewBag.ScreeningId = new SelectList(screenings, "Id", "Label");
    }
}
