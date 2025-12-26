using System.Globalization;
using System.Text;
using CinemaPlanner.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace CinemaPlanner.Web.Services;

public class BookingEventSubscriber
{
    private readonly ILogger<BookingEventSubscriber> _logger;
    private readonly CinemaPlannerDbContext _context;
    private readonly IReceiptStorage _receiptStorage;

    public BookingEventSubscriber(
        BookingService bookingService,
        ILogger<BookingEventSubscriber> logger,
        CinemaPlannerDbContext context,
        IReceiptStorage receiptStorage)
    {
        _logger = logger;
        _context = context;
        _receiptStorage = receiptStorage;

        bookingService.BookingCreated += HandleBookingCreated;
    }

    private void HandleBookingCreated(object? sender, BookingEventArgs e)
    {
        _logger.LogInformation(
            "Booking created: Id {BookingId} for screening {ScreeningId} seat {Row}-{Seat}.",
            e.Booking.Id,
            e.Booking.ScreeningId,
            e.Booking.SeatRow,
            e.Booking.SeatNumber);

        _ = GenerateReceiptAsync(e.Booking.Id);
    }

    private async Task GenerateReceiptAsync(int bookingId)
    {
        var start = DateTime.UtcNow;
        try
        {
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Screening)
                .ThenInclude(s => s!.Movie)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return;

            var receipt = BuildReceipt(booking);
            var bytes = Encoding.UTF8.GetBytes(receipt);
            await using var stream = new MemoryStream(bytes);

            var objectName = $"receipts/booking-{booking.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}.txt";
            await _receiptStorage.UploadReceiptAsync(objectName, "text/plain", stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate receipt for booking {BookingId}.", bookingId);
        }
        finally
        {
            var elapsedMs = (DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogDebug("Receipt generation finished for booking {BookingId} in {ElapsedMs} ms.", bookingId, elapsedMs);
        }
    }

    private static string BuildReceipt(Models.Booking booking)
    {
        var movieTitle = booking.Screening?.Movie?.Title ?? "Unknown";
        var startTime = booking.Screening?.StartTime.ToString("f", CultureInfo.InvariantCulture) ?? "TBD";
        var seatCode = CreateSeatCode(booking.SeatRow, booking.SeatNumber);
        var legacySeatCode = unchecked((int)seatCode);
        var seatCodeHex = seatCode.ToString("X");
        var statusFlags = (BookingStatusFlags)booking.StatusFlags;
        var isPaid = (statusFlags & BookingStatusFlags.Paid) == BookingStatusFlags.Paid;

        var sb = new StringBuilder();
        sb.AppendLine("CinemaPlanner Booking Receipt");
        sb.AppendLine($"Booking ID: {booking.Id}");
        sb.AppendLine($"Movie: {movieTitle}");
        sb.AppendLine($"Screening: {startTime}");
        sb.AppendLine($"Seat: Row {booking.SeatRow}, Seat {booking.SeatNumber}");
        sb.AppendLine($"Seat Code: 0x{seatCodeHex} ({seatCode.ToString(CultureInfo.InvariantCulture)})");
        sb.AppendLine($"Legacy Seat Code: {legacySeatCode}");
        sb.AppendLine($"Payment status: {(isPaid ? "Paid" : "Pending")}");
        sb.AppendLine($"Customer: {booking.CustomerName}");
        sb.AppendLine($"Created (UTC): {DateTime.UtcNow:O}");
        return sb.ToString();
    }

    private static long CreateSeatCode(int row, int seat)
    {
        long code = row << 32;
        code |= (uint)seat;
        return code;
    }
}
