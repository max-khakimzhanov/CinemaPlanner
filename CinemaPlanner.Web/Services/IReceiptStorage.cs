namespace CinemaPlanner.Web.Services;

public interface IReceiptStorage
{
    Task<string> UploadReceiptAsync(string objectName, string contentType, Stream content, CancellationToken cancellationToken = default);
}
