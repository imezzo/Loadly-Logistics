using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services.Media;




public static class MediaHelper
{








    public static string Read(this IMediaService mediaService, string directoryPath, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }
        var path = string.IsNullOrWhiteSpace(directoryPath)
            ? fileName
            : $"{directoryPath.TrimEnd('/')}/{fileName}";
        return mediaService.Read(path);
    }




    public static string ToMediaUrl(this string fileName, string directoryPath, IMediaService mediaService)
    {
        return mediaService.Read(directoryPath, fileName);
    }




    public static string ToMediaUrlOrEmpty(this string? fileName, string directoryPath, IMediaService mediaService)
    {
        return string.IsNullOrWhiteSpace(fileName)
            ? string.Empty
            : mediaService.Read(directoryPath, fileName);
    }
}
