using CinemaPlanner.Web.Services;
using Xunit;

namespace CinemaPlanner.Web.Tests.Services;

[Trait("Area", "SeatLayout")]
public class SeatLayoutServiceTests
{
    [Fact(DisplayName = "Given rows and seats, When building layout, Then dimensions match and aisles are marked")]
    public void GivenLayout_WhenBuild_ThenMatrixHasAisles()
    {
        var service = new SeatLayoutService();

        var result = service.BuildLayout(3, 10);

        Assert.Equal(3, result.SeatMatrix.GetLength(0));
        Assert.Equal(10, result.SeatMatrix.GetLength(1));
        Assert.Equal('-', result.SeatMatrix[0, 4]);
        Assert.Equal('-', result.SeatMatrix[2, 9]);
    }

    [Fact(DisplayName = "Given layout, When building, Then a preferred seat is marked")]
    public void GivenLayout_WhenBuild_ThenPreferredSeatMarked()
    {
        var service = new SeatLayoutService();

        var result = service.BuildLayout(5, 7);

        var marked = 0;
        for (int r = 0; r < result.SeatMatrix.GetLength(0); r++)
        {
            for (int c = 0; c < result.SeatMatrix.GetLength(1); c++)
            {
                if (result.SeatMatrix[r, c] == 'X')
                {
                    marked++;
                }
            }
        }

        Assert.Equal(1, marked);
    }
}
