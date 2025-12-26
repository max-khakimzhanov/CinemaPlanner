using System.Numerics;
using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Dtos;
using CinemaPlanner.Web.Mapping;
using CinemaPlanner.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class BookingAppService : IBookingAppService
{
    private readonly CinemaPlannerDbContext _context;
    private readonly BookingService _bookingService;
    private readonly BookingEventSubscriber _eventSubscriber;
    private readonly bool _bookingsDisabled;
    private readonly string _adminEmail;

    public BookingAppService(CinemaPlannerDbContext context, BookingService bookingService, IConfiguration configuration, BookingEventSubscriber eventSubscriber)
    {
        _context = context;
        _bookingService = bookingService;
        _eventSubscriber = eventSubscriber;
        _bookingsDisabled = bool.TryParse(configuration["CINEMA_FEATURE_FLAGS__DISABLE_BOOKINGS"], out var flag) && flag;
        _adminEmail = configuration["CINEMA_ADMIN_EMAIL"] ?? "(not set)";
    }

    public bool IsBookingDisabled => _bookingsDisabled;
    public string AdminEmail => _adminEmail;

    public async Task<IReadOnlyList<BookingListDto>> GetAllAsync()
    {
        var bookings = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Screening)
            .ThenInclude(s => s!.Movie)
            .ToListAsync();

        return bookings.Select(b => b.ToListDto()).ToList();
    }

    public async Task CreateAsync(BookingCreateDto dto)
    {
        var booking = new Booking
        {
            ScreeningId = dto.ScreeningId,
            SeatRow = dto.SeatRow,
            SeatNumber = dto.SeatNumber,
            CustomerName = dto.CustomerName,
            StatusFlags = (int)(BookingStatusFlags.Reserved | BookingStatusFlags.Paid)
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        _bookingService.CreateBooking(booking);
    }

    public async Task<IReadOnlyList<(int Id, string Label)>> GetScreeningOptionsAsync()
    {
        var screenings = await _context.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        return screenings.Select(s => (s.Id, $"{s.Movie?.Title} @ {s.StartTime:g} ({s.Hall?.Name})")).ToList();
    }

    public async Task<BigInteger?> GetSeatCombinationEstimateAsync()
    {
        var firstHall = await _context.Halls.AsNoTracking().FirstOrDefaultAsync();
        if (firstHall == null) return null;

        var totalSeats = firstHall.Rows * firstHall.SeatsPerRow;
        return Combination(totalSeats, 2);
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
