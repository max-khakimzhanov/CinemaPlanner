using CinemaPlanner.Web.Tests.Fixtures;
using Xunit;

namespace CinemaPlanner.Web.Tests;

[CollectionDefinition("SharedTestContext")]
public class SharedTestContextCollection : ICollectionFixture<SharedTestContext>
{
}
