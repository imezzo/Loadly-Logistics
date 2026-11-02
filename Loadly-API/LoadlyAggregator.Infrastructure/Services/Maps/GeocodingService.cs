using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Maps;

namespace LoadlyAggregator.Infrastructure.Services.Maps;

internal static class GeocodingService
{
    private const string NominatimUserAgent = "Loadly/1.0 (maps-extractor)";
    private const int NominatimDelayMs = 1100;
    private const int PhotonDelayMs = 300;

    public static async Task<bool> TryGeocodeAsync(
        string address,
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken = default)
    {
        var queries = new[] { address }
            .Concat(AddressFallbackQueryBuilder.BuildFallbackQueries(address))
            .Where(q => !string.IsNullOrWhiteSpace(q))
            .Distinct()
            .ToList();

        if (await TryWithNominatimAsync(queries, result, cancellationToken))
            return true;

        return await TryWithPhotonAsync(queries, result, cancellationToken);
    }

    public static async Task<bool> TryReverseGeocodeAsync(
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken = default)
    {
        if (result.Latitude is null || result.Longitude is null)
            return false;

        using var client = CreateHttpClient();
        var url = $"https://nominatim.openstreetmap.org/reverse?lat={result.Latitude}&lon={result.Longitude}&format=json";

        using var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("display_name", out var displayName))
            return false;

        result.Address = displayName.GetString()?.Trim();
        return true;
    }

    private static async Task<bool> TryWithNominatimAsync(
        IReadOnlyList<string> queries,
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken)
    {
        using var client = CreateHttpClient();

        foreach (var query in queries)
        {
            if (await TryNominatimGeocodeAsync(client, query, result, cancellationToken))
                return true;
            await Task.Delay(NominatimDelayMs, cancellationToken);
        }

        return false;
    }

    private static async Task<bool> TryWithPhotonAsync(
        IReadOnlyList<string> queries,
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken)
    {
        foreach (var query in queries)
        {
            if (await TryPhotonGeocodeAsync(query, result, cancellationToken))
                return true;
            await Task.Delay(PhotonDelayMs, cancellationToken);
        }

        return false;
    }

    private static async Task<bool> TryNominatimGeocodeAsync(
        HttpClient client,
        string query,
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken)
    {
        var encoded = Uri.EscapeDataString(query);
        var url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=json&limit=1";

        using var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
            return false;

        var first = root[0];
        if (!first.TryGetProperty("lat", out var latEl) || !first.TryGetProperty("lon", out var lonEl))
            return false;

        if (!decimal.TryParse(latEl.GetString(), out var lat) || !decimal.TryParse(lonEl.GetString(), out var lng))
            return false;

        if (lat < -90 || lat > 90 || lng < -180 || lng > 180)
            return false;

        result.Latitude = lat;
        result.Longitude = lng;
        return true;
    }

    private static async Task<bool> TryPhotonGeocodeAsync(
        string query,
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken)
    {
        var encoded = Uri.EscapeDataString(query);
        var url = $"https://photon.komoot.io/api/?q={encoded}&limit=1";

        using var client = CreateHttpClient();
        using var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("features", out var features) || features.GetArrayLength() == 0)
            return false;

        var first = features[0];
        if (!first.TryGetProperty("geometry", out var geom) || !geom.TryGetProperty("coordinates", out var coords))
            return false;

        if (coords.GetArrayLength() < 2)
            return false;

        // Photon returns [lon, lat]
        if (!decimal.TryParse(coords[0].ToString(), out var lng) || !decimal.TryParse(coords[1].ToString(), out var lat))
            return false;

        if (lat < -90 || lat > 90 || lng < -180 || lng > 180)
            return false;

        result.Latitude = lat;
        result.Longitude = lng;
        return true;
    }

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        client.DefaultRequestHeaders.Add("User-Agent", NominatimUserAgent);
        return client;
    }
}
