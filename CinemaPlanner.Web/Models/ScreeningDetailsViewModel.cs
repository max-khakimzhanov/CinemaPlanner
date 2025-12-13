namespace CinemaPlanner.Web.Models;

public class ScreeningDetailsViewModel
{
    public Screening Screening { get; set; } = default!;
    public char[,] SeatMatrix { get; set; } = default!;
    public string[][] SeatLabels { get; set; } = default!;
}
