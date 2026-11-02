using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class DocumentFileTypeValidator
{
    public static readonly string[] AllowedMimeTypes =
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp",
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain", "text/csv"
    };

    public static void Validate(string contentType)
    {
        _ = !string.IsNullOrWhiteSpace(contentType)
            ? true
            : throw new ValidationException("documents.file_content_type_required");

        _ = AllowedMimeTypes.Contains(contentType.Trim().ToLowerInvariant())
            ? true
            : throw new ValidationException("documents.file_type_not_allowed");
    }
}
