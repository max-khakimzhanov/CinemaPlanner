namespace CinemaPlanner.Web.Tests.TestData;

public static class BookingDummyData
{
    public static IEnumerable<object[]> NormalizationCases()
    {
        yield return ["   aLiCe   joHNsOn   ", "Alice Johnson"];
        yield return ["  MARIA  ivanovA ", "Maria Ivanova"];
        yield return ["bob smith", "Bob Smith"];
    }
}
