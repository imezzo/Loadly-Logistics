using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Media.StorageProviders;





public class S3StorageProvider : IStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<S3StorageProvider> _logger;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _region;
    private readonly string _cdnDomain;
    private readonly string _cdnMainFolder;

    public S3StorageProvider(IConfiguration configuration, ILogger<S3StorageProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _bucketName = _configuration["Media:AwsS3:BucketName"]
            ?? throw new InvalidOperationException("Media:AwsS3:BucketName not configured");
        _region = _configuration["Media:AwsS3:Region"]
            ?? throw new InvalidOperationException("Media:AwsS3:Region not configured");
        _cdnDomain = _configuration["Media:AwsS3:CdnDomain"]
            ?? _configuration["Media:ImagesServerDomain"]
            ?? throw new InvalidOperationException("Media:ImagesServerDomain not configured");
        _cdnMainFolder = _configuration["Media:CdnMainFolder"]
            ?? throw new InvalidOperationException("Media:CdnMainFolder not configured");


        var awsAccessKey = _configuration["Media:AwsS3:AccessKey"];
        var awsSecretKey = _configuration["Media:AwsS3:SecretKey"];
        // ponytail: Railway/Tigris needs ServiceURL; real AWS keeps RegionEndpoint
        var endpoint = _configuration["Media:AwsS3:Endpoint"];
        var forcePathStyle = string.Equals(
            _configuration["Media:AwsS3:ForcePathStyle"], "true", StringComparison.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint.TrimEnd('/'),
                ForcePathStyle = forcePathStyle,
                AuthenticationRegion = string.IsNullOrWhiteSpace(_region) ? "auto" : _region
            };

            _s3Client = string.IsNullOrWhiteSpace(awsAccessKey) || string.IsNullOrWhiteSpace(awsSecretKey)
                ? new AmazonS3Client(config)
                : new AmazonS3Client(awsAccessKey, awsSecretKey, config);
            _logger.LogInformation("S3 client initialized with custom endpoint {Endpoint}", endpoint);
        }
        else if (string.IsNullOrWhiteSpace(awsAccessKey) || string.IsNullOrWhiteSpace(awsSecretKey))
        {
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_region);
            _s3Client = new AmazonS3Client(regionEndpoint);
            _logger.LogInformation("S3 client initialized using default credential chain");
        }
        else
        {
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_region);
            _s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, regionEndpoint);
            _logger.LogInformation("S3 client initialized using explicit credentials");
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string path, string contentType)
    {
        var key = $"{_cdnMainFolder}/{path}/{fileName}".Replace('\\', '/');

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            await _s3Client.PutObjectAsync(request);

            _logger.LogInformation("File uploaded to S3: {Key} in bucket {Bucket}", key, _bucketName);

            return Path.Combine(path, fileName).Replace('\\', '/');
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to S3: {Key}", key);
            throw;
        }
    }

    public async Task DeleteAsync(string path)
    {
        var key = $"{_cdnMainFolder}/{path}".Replace('\\', '/');

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);

            _logger.LogInformation("File deleted from S3: {Key} in bucket {Bucket}", key, _bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from S3: {Key}", key);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string path)
    {
        var key = $"{_cdnMainFolder}/{path}".Replace('\\', '/');

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if file exists in S3: {Key}", key);
            return false;
        }
    }

    public string GetBaseUrl()
    {

        return _cdnDomain.TrimEnd('/');
    }

    public Task<string?> GetPresignedUrlAsync(string path, int expirationMinutes = 60)
    {
        var key = $"{_cdnMainFolder}/{path}".Replace('\\', '/');

        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };


            var presignedUrl = _s3Client.GetPreSignedURL(request);

            _logger.LogInformation("Generated presigned URL for S3 object: {Key}, expires in {Minutes} minutes",
                key, expirationMinutes);

            return Task.FromResult<string?>(presignedUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for S3: {Key}", key);
            return Task.FromResult<string?>(null);
        }
    }
}

