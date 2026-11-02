using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Media.StorageProviders;

public class LocalStorageProvider : IStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocalStorageProvider> _logger;
    private readonly string _storagePath;
    private readonly string _cdnMainFolder;
    private readonly string _cdnDomain;

    public LocalStorageProvider(IConfiguration configuration, ILogger<LocalStorageProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _cdnMainFolder = _configuration["Media:CdnMainFolder"]
            ?? throw new InvalidOperationException("Media:CdnMainFolder not configured");
        _cdnDomain = _configuration["Media:ImagesServerDomain"]
            ?? throw new InvalidOperationException("Media:ImagesServerDomain not configured");
        _storagePath = _configuration["Media:StoragePath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string path, string contentType)
    {
        Directory.CreateDirectory(Path.Combine(_storagePath, _cdnMainFolder, path));
        await CopyStreamToFileAsync(
            fileStream.CanSeek ? ResetPosition(fileStream) : await BufferToMemoryAsync(fileStream),
            Path.Combine(_storagePath, _cdnMainFolder, path, fileName));
        _logger.LogInformation("File uploaded to local storage: {FilePath}", Path.Combine(path, fileName));
        return Path.Combine(path, fileName).Replace('\\', '/');
    }

    public Task DeleteAsync(string path) =>
        Task.Run(() => RunWhenExists(Path.Combine(_storagePath, _cdnMainFolder, path), DeleteAndLog));

    public Task<bool> ExistsAsync(string path) =>
        Task.FromResult(File.Exists(Path.Combine(_storagePath, _cdnMainFolder, path)));

    public string GetBaseUrl() => _cdnDomain.TrimEnd('/');

    public Task<string?> GetPresignedUrlAsync(string path, int expirationMinutes = 60) =>
        Task.FromResult<string?>(null);

    private static Stream ResetPosition(Stream stream)
    {
        stream.Position = 0;
        return stream;
    }

    private static async Task<Stream> BufferToMemoryAsync(Stream stream)
    {
        var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        memory.Position = 0;
        return memory;
    }

    private static async Task CopyStreamToFileAsync(Stream source, string filePath)
    {
        await using var output = new FileStream(filePath, FileMode.Create);
        try
        {
            await source.CopyToAsync(output);
        }
        catch (InvalidOperationException)
        {
            source.Position = 0;
            await using var fallback = new MemoryStream();
            await source.CopyToAsync(fallback);
            fallback.Position = 0;
            output.Position = 0;
            output.SetLength(0);
            await fallback.CopyToAsync(output);
        }
        finally
        {
            (source as MemoryStream)?.Dispose();
        }
    }

    private static void RunWhenExists(string fullPath, Action<string> action) =>
        (File.Exists(fullPath) ? (Action)(() => action(fullPath)) : (Action)(() => { }))();

    private void DeleteAndLog(string fullPath)
    {
        try
        {
            File.Delete(fullPath);
            _logger.LogInformation("File deleted from local storage: {FilePath}", fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", fullPath);
            throw;
        }
    }
}

