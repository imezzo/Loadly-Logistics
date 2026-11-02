namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Shared path logic for contractor document storage and resolution (DRY).
/// </summary>
public static class ContractorDocumentPathHelper
{
    public static string NormalizeDocumentName(string documentNameEn) =>
        documentNameEn.ToLower().Replace(" ", "-");

    /// <summary>
    /// Builds the directory path for a contractor document (e.g. "contractor/123/document-name").
    /// </summary>
    public static string BuildDirectoryPath(long contractorId, string documentNameEn) =>
        $"contractor/{contractorId}/{NormalizeDocumentName(documentNameEn)}";

    /// <summary>
    /// Returns the full path for storage or for media read/delete.
    /// If <paramref name="fileNameOrPath"/> already contains '/' it is treated as a full path; otherwise it is appended to the directory.
    /// </summary>
    public static string BuildFilePath(long contractorId, string documentNameEn, string fileNameOrPath)
    {
        if (fileNameOrPath.Contains('/'))
            return fileNameOrPath;
        return $"{BuildDirectoryPath(contractorId, documentNameEn)}/{fileNameOrPath}";
    }
}
