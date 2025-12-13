namespace CinemaPlanner.Web.Models;

public readonly struct SeatPosition(int row, int seat)
{
    public int Row { get; } = row;
    public int Seat { get; } = seat;

    public static SeatPosition operator +(SeatPosition left, SeatPosition right) =>
        new(left.Row + right.Row, left.Seat + right.Seat);

    public override string ToString() => $"Row {Row}, Seat {Seat}";
}
