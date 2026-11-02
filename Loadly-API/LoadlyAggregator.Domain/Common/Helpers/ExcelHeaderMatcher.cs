namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ExcelHeaderMatcher
{
    public static bool Matches(IReadOnlyList<string>? actualHeaders, IReadOnlyList<string> expectedHeaders)
    {
        if (actualHeaders == null || actualHeaders.Count != expectedHeaders.Count)
            return false;

        return actualHeaders
            .Zip(expectedHeaders, (a, e) => string.Equals(Normalize(a), Normalize(e), StringComparison.OrdinalIgnoreCase))
            .All(eq => eq);
    }

    private static string Normalize(string? header) =>
        string.IsNullOrEmpty(header) ? string.Empty : header.Trim().Replace('\u00A0', ' ');
}
