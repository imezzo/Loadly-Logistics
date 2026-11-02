using System.Text.RegularExpressions;

namespace LoadlyAggregator.Infrastructure.Services.Maps;


internal static class AddressFallbackQueryBuilder
{
    private static readonly Regex GovernorateRegex = new(
        @"([A-Za-z\u0600-\u06FF\s]+?)\s+Governorate(?:\s+\d+)?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);


    public static IEnumerable<string> BuildFallbackQueries(string address)
    {
        foreach (var query in GetCommaBasedFallbacks(address))
            yield return query;

        foreach (var query in GetEgyptianFallbacks(address))
            yield return query;
    }

    private static IEnumerable<string> GetCommaBasedFallbacks(string address)
    {
        var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
            yield return string.Join(", ", parts[^2], parts[^1]);
        if (parts.Length >= 3)
            yield return string.Join(", ", parts[^3], parts[^2], parts[^1]);
        if (parts.Length >= 4)
            yield return string.Join(", ", parts[^4], parts[^3], parts[^2], parts[^1]);
    }

    private static IEnumerable<string> GetEgyptianFallbacks(string address)
    {
        var match = GovernorateRegex.Match(address);
        if (!match.Success)
            yield break;

        var governorate = match.Groups[1].Value.Trim();
        if (string.IsNullOrWhiteSpace(governorate))
            yield break;

        yield return $"{governorate} Governorate, Egypt";
    }
}
