namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Shared path logic for contract document storage and resolution (DRY).
/// </summary>
public static class ContractDocumentPathHelper
{
    public static string NormalizeDocumentName(string documentNameEn) =>
        documentNameEn.ToLower().Replace(" ", "-");

    /// <summary>
    /// Builds the directory path for a contract document (e.g. "contract/123/document-name").
    /// </summary>
    public static string BuildDirectoryPath(long contractId, string documentNameEn) =>
        $"contract/{contractId}/{NormalizeDocumentName(documentNameEn)}";

    /// <summary>
    /// Returns the full path for storage or for media read/delete.
    /// If <paramref name="fileNameOrPath"/> already contains '/' it is treated as a full path; otherwise it is appended to the directory.
    /// </summary>
    public static string BuildFilePath(long contractId, string documentNameEn, string fileNameOrPath)
    {
        if (fileNameOrPath.Contains('/'))
            return fileNameOrPath;
        return $"{BuildDirectoryPath(contractId, documentNameEn)}/{fileNameOrPath}";
    }
}
