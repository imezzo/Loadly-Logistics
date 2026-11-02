using System.Text.RegularExpressions;
using LoadlyAggregator.Domain.Admin.DTOs.Maps;

namespace LoadlyAggregator.Infrastructure.Services.Maps;

/// <summary>
/// Parses Google Maps URLs to extract coordinates, place ID, and address.
/// </summary>
internal static class GoogleMapsUrlParser
{
    private static readonly Regex LatLngAtRegex = new(
        @"@(-?\d+\.?\d*),(-?\d+\.?\d*)",
        RegexOptions.Compiled);

    private static readonly Regex LatLng3d4dRegex = new(
        @"!3d(-?\d+\.?\d*)!4d(-?\d+\.?\d*)",
        RegexOptions.Compiled);

    private static readonly Regex LatLng1d2dRegex = new(
        @"!1d(-?\d+\.?\d*)!2d(-?\d+\.?\d*)",
        RegexOptions.Compiled);

    private static readonly Regex LatLngQueryRegex = new(
        @"[?&]q=(-?\d+\.?\d*),(-?\d+\.?\d*)",
        RegexOptions.Compiled);

    private static readonly Regex LatLngCommonQueryRegex = new(
        @"[?&](?:query|ll|sll|center|destination|daddr)=([+-]?\d+\.?\d*)(?:,|%2C)([+-]?\d+\.?\d*)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex LatLngSearchPathRegex = new(
        @"/maps/search/([+-]?\d+\.?\d*)\s*,\s*\+?([+-]?\d+\.?\d*)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PlaceIdChIjRegex = new(
        @"ChIJ[A-Za-z0-9_-]+",
        RegexOptions.Compiled);

    private static readonly Regex PlaceIdHexRegex = new(
        @"0x[0-9a-f]+:0x[0-9a-f]+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex AddressFromPlacePathRegex = new(
        @"/place/([^/]+?)(?:/data=|\?|@|$)",
        RegexOptions.Compiled);

    // Order matters: the first match is kept. !3d!4d is the place pin.
    // @lat,lng is only the map camera and can be kilometers away from that pin.
    private static readonly (Regex Regex, int LatGroup, int LngGroup)[] LatLngPatterns =
    {
        (LatLng3d4dRegex, 1, 2),
        (LatLngQueryRegex, 1, 2),
        (LatLngCommonQueryRegex, 1, 2),
        (LatLngSearchPathRegex, 1, 2),
        (LatLngAtRegex, 1, 2),
        (LatLng1d2dRegex, 2, 1)
    };

    public static void ExtractFromUrl(string url, GoogleMapsPlaceDto result)
    {
        TryExtractLatLng(url, result);
        TryExtractPlaceId(url, result);
        TryExtractAddressFromPlacePath(url, result);
    }

    public static bool IsShortUrl(string url)
    {
        return url.Contains("goo.gl/maps", StringComparison.OrdinalIgnoreCase)
            || url.Contains("maps.app.goo.gl", StringComparison.OrdinalIgnoreCase);
    }

    private static void TryExtractLatLng(string url, GoogleMapsPlaceDto result)
    {
        if (result.Latitude is not null && result.Longitude is not null)
            return;

        foreach (var (regex, latGroup, lngGroup) in LatLngPatterns)
        {
            var match = regex.Match(url);
            if (!match.Success)
                continue;

            if (!decimal.TryParse(match.Groups[latGroup].Value, out var lat)
                || !decimal.TryParse(match.Groups[lngGroup].Value, out var lng))
                continue;

            if (lat < -90 || lat > 90 || lng < -180 || lng > 180)
                continue;

            result.Latitude = lat;
            result.Longitude = lng;
            return;
        }
    }

    private static void TryExtractPlaceId(string url, GoogleMapsPlaceDto result)
    {
        if (!string.IsNullOrEmpty(result.PlaceId))
            return;

        var chIjMatch = PlaceIdChIjRegex.Match(url);
        if (chIjMatch.Success)
        {
            result.PlaceId = chIjMatch.Value;
            return;
        }

        var hexMatch = PlaceIdHexRegex.Match(url);
        if (hexMatch.Success)
            result.PlaceId = hexMatch.Value;
    }

    private static void TryExtractAddressFromPlacePath(string url, GoogleMapsPlaceDto result)
    {
        if (!string.IsNullOrEmpty(result.Address))
            return;

        var match = AddressFromPlacePathRegex.Match(url);
        if (!match.Success)
            return;

        var encoded = match.Groups[1].Value;
        if (string.IsNullOrWhiteSpace(encoded))
            return;

        try
        {
            result.Address = Uri.UnescapeDataString(encoded).Replace('+', ' ').Trim();
        }
        catch
        {
            // Ignore decode errors
        }
    }
}
