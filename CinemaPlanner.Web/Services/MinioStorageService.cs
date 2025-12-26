using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System.Globalization;

namespace CinemaPlanner.Web.Services;

public class MinioStorageService : IPosterStorage, IReceiptStorage
{
    private readonly MinioOptions _options;
    private readonly IMinioClient _client;
    private bool _bucketReady;

    public MinioStorageService(IOptions<MinioOptions> options)
    {
        _options = options.Value;

        _client = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.UseSSL)
            .Build();
    }

    public IMinioClient Client => _client;
    public string BucketName => _options.BucketName;
    public MinioOptions Options => _options;

    public async Task<string> UploadPosterAsync(IFormFile file, string objectName, CancellationToken cancellationToken = default)
    {
        await EnsureBucketAsync(cancellationToken);

        using var stream = file.OpenReadStream();
        var objectKey = SanitizeObjectName(objectName);
        var putArgs = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(file.ContentType);

        await _client.PutObjectAsync(putArgs, cancellationToken);
        return BuildObjectUrl(objectKey);
    }

    public async Task<(Stream Content, string ContentType)> DownloadAsync(string objectName, CancellationToken cancellationToken = default)
    {
        await EnsureBucketAsync(cancellationToken);

        var stat = await _client.StatObjectAsync(new StatObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName), cancellationToken);

        var ms = new MemoryStream();
        var getArgs = new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyToAsync(ms));

        await _client.GetObjectAsync(getArgs, cancellationToken);
        ms.Position = 0;
        var contentType = stat.ContentType ?? "application/octet-stream";
        return (ms, contentType);
    }

    public async Task<string> UploadReceiptAsync(string objectName, string contentType, Stream content, CancellationToken cancellationToken = default)
    {
        await EnsureBucketAsync(cancellationToken);

        var objectKey = SanitizeObjectName(objectName);
        var putArgs = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await _client.PutObjectAsync(putArgs, cancellationToken);
        return BuildObjectUrl(objectKey);
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        if (_bucketReady)
        {
            return;
        }

        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_options.BucketName), cancellationToken);
        if (!exists)
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_options.BucketName), cancellationToken);
        }

        _bucketReady = true;
    }

    private string BuildObjectUrl(string objectName)
    {
        var endpoint = _options.Endpoint?.Trim();
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return objectName;
        }

        if (!endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var scheme = _options.UseSSL ? "https" : "http";
            endpoint = $"{scheme}://{endpoint}";
        }

        return $"{endpoint.TrimEnd('/')}/{_options.BucketName}/{objectName}";
    }

    private static string SanitizeObjectName(string name)
    {
        return name.Replace(' ', '-');
    }
}
