namespace CinemaPlanner.Web.Delegates;

public delegate TResult Mapper<in T, out TResult>(T value);
