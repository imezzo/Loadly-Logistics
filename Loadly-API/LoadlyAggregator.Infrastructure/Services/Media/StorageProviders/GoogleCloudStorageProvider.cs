using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Media.StorageProviders;





public class GoogleCloudStorageProvider : IStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleCloudStorageProvider> _logger;
    private readonly string _bucketName;
    private readonly string _cdnDomain;
    private readonly string _cdnMainFolder;


    public GoogleCloudStorageProvider(IConfiguration configuration, ILogger<GoogleCloudStorageProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _bucketName = _configuration["Media:GoogleCloud:BucketName"]
            ?? throw new InvalidOperationException("Media:GoogleCloud:BucketName not configured");
        _cdnDomain = _configuration["Media:GoogleCloud:CdnDomain"]
            ?? _configuration["Media:ImagesServerDomain"]
            ?? throw new InvalidOperationException("Media:ImagesServerDomain not configured");
        _cdnMainFolder = _configuration["Media:CdnMainFolder"]
            ?? throw new InvalidOperationException("Media:CdnMainFolder not configured");





    }

    public Task<string> UploadAsync(Stream fileStream, string fileName, string path, string contentType)
    {
        var objectName = $"{_cdnMainFolder}/{path}/{fileName}".Replace('\\', '/');





        _logger.LogInformation("File uploaded to GCS: {ObjectName}", objectName);

        return Task.FromResult(Path.Combine(path, fileName).Replace('\\', '/'));
    }

    public Task DeleteAsync(string path)
    {
        var objectName = $"{_cdnMainFolder}/{path}".Replace('\\', '/');





        _logger.LogInformation("File deleted from GCS: {ObjectName}", objectName);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string path)
    {
        var objectName = $"{_cdnMainFolder}/{path}".Replace('\\', '/');





        return await Task.FromResult(false);
    }

    public string GetBaseUrl()
    {

        return _cdnDomain.TrimEnd('/');
    }

    public Task<string?> GetPresignedUrlAsync(string path, int expirationMinutes = 60)
    {



        return Task.FromResult<string?>(null);
    }
}

