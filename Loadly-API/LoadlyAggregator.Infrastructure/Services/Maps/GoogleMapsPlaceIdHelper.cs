using System.Text.RegularExpressions;

namespace LoadlyAggregator.Infrastructure.Services.Maps;

internal static class GoogleMapsPlaceIdHelper
{
    private static readonly Regex HexPlaceIdRegex = new(
        @"^0x[0-9a-f]+:0x[0-9a-f]+$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool IsResolvableWithPlaceDetailsApi(string? placeId)
    {
        if (string.IsNullOrWhiteSpace(placeId))
            return false;

        return !HexPlaceIdRegex.IsMatch(placeId.Trim());
    }
}
