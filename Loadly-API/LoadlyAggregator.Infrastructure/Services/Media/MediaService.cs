using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Infrastructure.Services.Media;







public class MediaService : IMediaService
{
    private readonly IStorageProvider _storageProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MediaService> _logger;
    private readonly string _cdnMainFolder;

    public MediaService(
        IStorageProvider storageProvider,
        IConfiguration configuration,
        ILogger<MediaService> logger)
    {
        _storageProvider = storageProvider;
        _configuration = configuration;
        _logger = logger;

        _cdnMainFolder = _configuration["Media:CdnMainFolder"]
            ?? throw new InvalidOperationException("Media:CdnMainFolder not configured");
    }





    public async Task<string> UploadAsync(FileUpload file, string path)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null", nameof(file));
        }


        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var guid = Guid.NewGuid().ToString("N")[..8];
        var originalFileName = file.FileName;
        var extension = Path.GetExtension(originalFileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);


        var sanitizedFileName = SanitizeFileName(fileNameWithoutExtension);
        var fileName = $"{timestamp}_{guid}_{sanitizedFileName}{extension}";


        var storedPath = await _storageProvider.UploadAsync(
            file.Content,
            fileName,
            path,
            file.ContentType
        );

        _logger.LogInformation("File uploaded successfully: {FilePath}", storedPath);


        return fileName;
    }

    public async Task<string> UploadAndGetPathAsync(FileUpload file, string directoryPath)
    {
        var fileName = await UploadAsync(file, directoryPath);
        return $"{directoryPath}/{fileName}";
    }

    public async Task<List<string>> UploadImagesAsync(IEnumerable<FileUpload> images, string directoryPath)
    {
        var paths = new List<string>();
        foreach (var image in images ?? [])
        {
            if (image.Length > 0)
            {
                var path = await UploadAndGetPathAsync(image, directoryPath);
                paths.Add(path);
            }
        }
        return paths;
    }

    public string Read(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }


        path = path.TrimStart('/');


        var baseUrl = _storageProvider.GetBaseUrl().TrimEnd('/');
        var folder = _cdnMainFolder.Trim('/');
        var filePath = path.TrimStart('/');

        return $"{baseUrl}/{folder}/{filePath}";
    }




    public async Task<string> ReadSecureAsync(string path, int expirationMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }


        path = path.TrimStart('/');


        var presignedUrl = await _storageProvider.GetPresignedUrlAsync(path, expirationMinutes);

        if (!string.IsNullOrWhiteSpace(presignedUrl))
        {
            _logger.LogInformation("Using presigned URL for secure file access: {Path}", path);
            return presignedUrl;
        }


        return Read(path);
    }




    public async Task RemoveAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        await _storageProvider.DeleteAsync(path);
    }




    public Task<bool> ExistsAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Task.FromResult(false);
        }

        return _storageProvider.ExistsAsync(path);
    }




    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }
}

