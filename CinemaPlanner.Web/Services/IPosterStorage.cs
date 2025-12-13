namespace CinemaPlanner.Web.Services;

public interface IPosterStorage
{
    Task<string> UploadPosterAsync(IFormFile file, string objectName, CancellationToken cancellationToken = default);
    Task<(Stream Content, string ContentType)> DownloadAsync(string objectName, CancellationToken cancellationToken = default);
}
