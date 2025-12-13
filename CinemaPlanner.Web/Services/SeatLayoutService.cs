using CinemaPlanner.Web.Models;

namespace CinemaPlanner.Web.Services;

public record SeatLayoutResult(char[,] SeatMatrix, string[][] SeatLabels);

public class SeatLayoutService
{
    public SeatLayoutResult BuildLayout(int rows, int seatsPerRow)
    {
        var seatMatrix = new char[rows, seatsPerRow];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < seatsPerRow; c++)
            {
                ref char seatRef = ref GetSeatRef(seatMatrix, r, c);
                seatRef = 'O';
            }
        }

        var aisleSeatIndex = 4;
        while (aisleSeatIndex < seatsPerRow)
        {
            for (int r = 0; r < rows; r++)
            {
                seatMatrix[r, aisleSeatIndex] = '-';
            }
            aisleSeatIndex += 5;
        }

        var middleRow = Math.Max(0, rows / 2);
        var middleSeat = Math.Max(0, seatsPerRow / 2);
        var placed = 0;
        var offset = 0;
        do
        {
            var candidate = (middleSeat + offset) % seatsPerRow;
            ref char targetSeat = ref GetSeatRef(seatMatrix, middleRow, candidate);
            if (targetSeat == 'O')
            {
                targetSeat = 'X';
                placed++;
            }
            offset++;
        } while (placed == 0 && offset < seatsPerRow);

        var seatLabels = new string[rows][];
        for (int r = 0; r < rows; r++)
        {
            seatLabels[r] = new string[seatsPerRow];
            for (int c = 0; c < seatsPerRow; c++)
            {
                var rowLabel = (char)('A' + r);
                seatLabels[r][c] = $"{rowLabel}{c + 1}";
            }
        }

        return new SeatLayoutResult(seatMatrix, seatLabels);
    }

    private static ref char GetSeatRef(char[,] matrix, int row, int seat) => ref matrix[row, seat];
}
