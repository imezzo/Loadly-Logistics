namespace LoadlyAggregator.Domain.Common.Models;





public class FileUpload
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required long Length { get; init; }
    public required Stream Content { get; init; }




    public async Task<byte[]> ReadAsBytesAsync()
    {
        using var memoryStream = new MemoryStream();
        await Content.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }




    public bool IsWithinSizeLimit(long maxSizeInBytes) => Length <= maxSizeInBytes;




    public bool IsAllowedType(params string[] allowedMimeTypes)
        => allowedMimeTypes.Contains(ContentType.ToLower());




    public string GetFileExtension() => Path.GetExtension(FileName);
}

