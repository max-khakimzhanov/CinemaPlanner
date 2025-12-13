namespace CinemaPlanner.Web.Services;

public class MinioOptions
{
    public string Endpoint { get; set => field = value?.Trim() ?? string.Empty; } = string.Empty;
    public string AccessKey { get; set => field = value ?? string.Empty; } = string.Empty;
    public string SecretKey { get; set => field = value ?? string.Empty; } = string.Empty;
    public string BucketName { get; set => field = value ?? string.Empty; } = string.Empty;
    public bool UseSSL { get; set; }
}
