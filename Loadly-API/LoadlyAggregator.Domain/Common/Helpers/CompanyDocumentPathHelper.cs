namespace LoadlyAggregator.Domain.Common.Helpers;

public static class CompanyDocumentPathHelper
{

    public static string BuildFilePath(long companyId, string documentNameEn, string fileNameOrPath)
    {
        var normalizedName = documentNameEn.ToLower().Replace(" ", "-");
        if (fileNameOrPath.Contains('/'))
            return fileNameOrPath;
        return $"company/{companyId}/{normalizedName}/{fileNameOrPath}";
    }
}
