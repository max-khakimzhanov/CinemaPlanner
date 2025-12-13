using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CinemaPlanner.Web.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new ContentResult
        {
            Content = "A friendly error occurred while processing your request.",
            StatusCode = StatusCodes.Status500InternalServerError,
            ContentType = "text/plain"
        };
        context.ExceptionHandled = true;
    }
}
