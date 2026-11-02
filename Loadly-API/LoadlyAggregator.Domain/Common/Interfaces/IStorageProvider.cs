namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface IStorageProvider
{








    Task<string> UploadAsync(Stream fileStream, string fileName, string path, string contentType);




    Task DeleteAsync(string path);





    Task<bool> ExistsAsync(string path);




    string GetBaseUrl();






    Task<string?> GetPresignedUrlAsync(string path, int expirationMinutes = 60);
}
