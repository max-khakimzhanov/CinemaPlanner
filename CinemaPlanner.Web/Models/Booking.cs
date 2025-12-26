namespace CinemaPlanner.Web.Models;

public class Booking
{
    public int Id { get; set; }
    public int ScreeningId { get; set; }
    public int SeatRow { get; set; }
    public int SeatNumber { get; set; }
    public int StatusFlags { get; set; }
    public string CustomerName { get; set => field = value ?? throw new ArgumentNullException(nameof(value)); } = string.Empty;

    public Screening? Screening { get; set; }
}
