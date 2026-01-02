namespace CinemaPlanner.Web.Exceptions;

public class CinemaPlannerException : Exception
{
    public CinemaPlannerException(string message) : base(message)
    {
    }

    public CinemaPlannerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
