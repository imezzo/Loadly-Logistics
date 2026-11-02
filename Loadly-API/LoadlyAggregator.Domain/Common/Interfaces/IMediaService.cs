using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IMediaService
{
    Task<string> UploadAsync(FileUpload file, string path);
    Task<string> UploadAndGetPathAsync(FileUpload file, string directoryPath);
    Task<List<string>> UploadImagesAsync(IEnumerable<FileUpload> images, string directoryPath);
    string Read(string path);
    Task<string> ReadSecureAsync(string path, int expirationMinutes = 60);
    Task RemoveAsync(string path);
    Task<bool> ExistsAsync(string path);
}
