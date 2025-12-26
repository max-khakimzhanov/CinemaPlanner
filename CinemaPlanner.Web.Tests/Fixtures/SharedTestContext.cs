namespace CinemaPlanner.Web.Tests.Fixtures;

public class SharedTestContext : IDisposable
{
    public DateTime StartedUtc { get; } = DateTime.UtcNow;

    public void Dispose()
    {
    }
}
