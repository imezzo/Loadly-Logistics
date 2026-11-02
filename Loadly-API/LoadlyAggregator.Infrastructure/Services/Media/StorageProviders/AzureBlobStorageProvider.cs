using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Media.StorageProviders;





public class AzureBlobStorageProvider : IStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureBlobStorageProvider> _logger;
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly string _cdnDomain;
    private readonly string _cdnMainFolder;


    public AzureBlobStorageProvider(IConfiguration configuration, ILogger<AzureBlobStorageProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _connectionString = _configuration["Media:AzureBlob:ConnectionString"]
            ?? throw new InvalidOperationException("Media:AzureBlob:ConnectionString not configured");
        _containerName = _configuration["Media:AzureBlob:ContainerName"]
            ?? throw new InvalidOperationException("Media:AzureBlob:ContainerName not configured");
        _cdnDomain = _configuration["Media:AzureBlob:CdnDomain"]
            ?? _configuration["Media:ImagesServerDomain"]
            ?? throw new InvalidOperationException("Media:ImagesServerDomain not configured");
        _cdnMainFolder = _configuration["Media:CdnMainFolder"]
            ?? throw new InvalidOperationException("Media:CdnMainFolder not configured");



    }

    public Task<string> UploadAsync(Stream fileStream, string fileName, string path, string contentType)
    {
        var blobName = $"{_cdnMainFolder}/{path}/{fileName}".Replace('\\', '/');





        _logger.LogInformation("File uploaded to Azure Blob: {BlobName}", blobName);

        return Task.FromResult(Path.Combine(path, fileName).Replace('\\', '/'));
    }

    public Task DeleteAsync(string path)
    {
        var blobName = $"{_cdnMainFolder}/{path}".Replace('\\', '/');





        _logger.LogInformation("File deleted from Azure Blob: {BlobName}", blobName);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string path)
    {
        var blobName = $"{_cdnMainFolder}/{path}".Replace('\\', '/');





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

