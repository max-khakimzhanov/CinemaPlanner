using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Models;
using CinemaPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace CinemaPlanner.Web.Controllers;

[Route("booking/[action]")]
public class BookingController : Controller
{
    private readonly CinemaPlannerDbContext _context;
    private readonly BookingService _bookingService;
    private static bool _subscribed = false;
    private readonly bool _bookingsDisabled;
    private readonly string? _adminEmail;

    public BookingController(
        CinemaPlannerDbContext context,
        BookingService bookingService,
        IConfiguration configuration)
    {
        _context = context;
        _bookingService = bookingService;
        _bookingsDisabled = bool.TryParse(configuration["CINEMA_FEATURE_FLAGS__DISABLE_BOOKINGS"], out var flag) && flag;
        _adminEmail = configuration["CINEMA_ADMIN_EMAIL"];

        if (!_subscribed)
        {
            _bookingService.BookingCreated += (_, args) =>
            {
            };
            _subscribed = true;
        }
    }

    [HttpGet("/booking")]
    [HttpGet("/booking/index")]
    public async Task<IActionResult> Index()
    {
        var bookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Screening)
            .ThenInclude(s => s!.Movie)
            .ToListAsync();

        var firstHall = await _context.Halls.AsNoTracking().FirstOrDefaultAsync();
        BigInteger? combinations = null;
        if (firstHall != null)
        {
            var totalSeats = firstHall.Rows * firstHall.SeatsPerRow;
            combinations = Combination(totalSeats, 2);
        }

        ViewBag.SeatCombinationEstimate = combinations;
        ViewBag.BookingsDisabled = _bookingsDisabled;
        ViewBag.AdminEmail ??= "(not set)";
        var uploadSuccessValue = TempData.Peek("ReceiptUploadSuccess");
        bool uploadSuccess = uploadSuccessValue switch
        {
            bool b => b,
            string s when bool.TryParse(s, out var parsed) => parsed,
            _ => false
        };
        ViewBag.ReceiptUploadSuccess = uploadSuccess;
        ViewBag.ReceiptUploadMessage = TempData.Peek("ReceiptUploadMessage") as string;
        ViewBag.ReceiptUploadUrl = TempData.Peek("ReceiptUploadUrl") as string;

        return View(bookings);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (_bookingsDisabled)
        {
            return Forbid("Bookings are disabled via configuration.");
        }
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ScreeningId,SeatRow,SeatNumber,CustomerName")] Booking booking)
    {
        if (_bookingsDisabled)
        {
            return Forbid("Bookings are disabled via configuration.");
        }
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(booking);
        }

        booking.StatusFlags = (int)(BookingStatusFlags.Reserved | BookingStatusFlags.Paid);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        _bookingService.CreateBooking(booking);

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        var screenings = await _context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        ViewBag.ScreeningId = new SelectList(
            screenings.Select(s => new
            {
                s.Id,
                Label = $"{s.Movie?.Title} @ {s.StartTime:g} ({s.Hall?.Name})"
            }),
            "Id",
            "Label");
    }

    private static BigInteger Combination(int n, int k)
    {
        if (k < 0 || k > n) return BigInteger.Zero;
        if (k == 0 || k == n) return BigInteger.One;
        k = Math.Min(k, n - k);
        BigInteger result = BigInteger.One;
        for (int i = 1; i <= k; i++)
        {
            result *= n - (k - i);
            result /= i;
        }
        return result;
    }
}
