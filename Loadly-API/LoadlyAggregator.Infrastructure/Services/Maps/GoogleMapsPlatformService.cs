using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Admin.DTOs.Maps;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Options;

namespace LoadlyAggregator.Infrastructure.Services.Maps;

public class GoogleMapsPlatformService : IGoogleMapsPlatformService
{
    private const string GeocodeBaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    private const string PlaceDetailsBaseUrl = "https://maps.googleapis.com/maps/api/place/details/json";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleMapsOptions _options;
    private readonly ILogger<GoogleMapsPlatformService> _logger;

    public GoogleMapsPlatformService(
        IHttpClientFactory httpClientFactory,
        IOptions<GoogleMapsOptions> options,
        ILogger<GoogleMapsPlatformService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey);

    public async Task<bool> TryEnrichFromPlaceIdAsync(
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || !GoogleMapsPlaceIdHelper.IsResolvableWithPlaceDetailsApi(result.PlaceId))
            return false;

        var placeId = result.PlaceId!.Trim();
        var url =
            $"{PlaceDetailsBaseUrl}?place_id={Uri.EscapeDataString(placeId)}"
            + "&fields=geometry,formatted_address,place_id"
            + $"&key={Uri.EscapeDataString(_options.ApiKey)}";

        var json = await GetApiJsonAsync(url, cancellationToken);
        if (json is null)
            return false;

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!IsOkStatus(root, "Place Details", placeId))
            return false;

        if (!root.TryGetProperty("result", out var placeResult))
            return false;

        if (!TryApplyGeometry(placeResult, result))
            return false;

        if (placeResult.TryGetProperty("formatted_address", out var formattedAddress))
        {
            var address = formattedAddress.GetString()?.Trim();
            if (!string.IsNullOrEmpty(address))
                result.Address = address;
        }

        if (placeResult.TryGetProperty("place_id", out var placeIdElement))
        {
            var resolvedPlaceId = placeIdElement.GetString()?.Trim();
            if (!string.IsNullOrEmpty(resolvedPlaceId))
                result.PlaceId = resolvedPlaceId;
        }

        return true;
    }

    public async Task<bool> TryGeocodeAddressAsync(
        string address,
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(address))
            return false;

        var url = BuildGeocodeUrl(address, lat: null, lng: null);
        var json = await GetApiJsonAsync(url, cancellationToken);
        if (json is null)
            return false;

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!IsOkStatus(root, "Geocode", address))
            return false;

        return TryApplyFirstGeocodeResult(root, result);
    }

    public async Task<bool> TryReverseGeocodeAsync(
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || result.Latitude is null || result.Longitude is null)
            return false;

        var url = BuildGeocodeUrl(address: null, result.Latitude, result.Longitude);
        var json = await GetApiJsonAsync(url, cancellationToken);
        if (json is null)
            return false;

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!IsOkStatus(root, "Reverse geocode", $"{result.Latitude},{result.Longitude}"))
            return false;

        if (!TryApplyFirstGeocodeResult(root, result, updateCoordinates: false))
            return false;

        return !string.IsNullOrEmpty(result.Address);
    }

    private string BuildGeocodeUrl(string? address, decimal? lat, decimal? lng)
    {
        var query = address is not null
            ? $"address={Uri.EscapeDataString(address)}"
            : $"latlng={lat!.Value.ToString(CultureInfo.InvariantCulture)},{lng!.Value.ToString(CultureInfo.InvariantCulture)}";

        var url = $"{GeocodeBaseUrl}?{query}&key={Uri.EscapeDataString(_options.ApiKey)}";

        if (address is not null && !string.IsNullOrWhiteSpace(_options.DefaultCountryCode))
        {
            var region = _options.DefaultCountryCode.Trim().ToLowerInvariant();
            url += $"&region={Uri.EscapeDataString(region)}";
            url += $"&components=country:{Uri.EscapeDataString(_options.DefaultCountryCode.Trim().ToUpperInvariant())}";
        }

        return url;
    }

    private async Task<string?> GetApiJsonAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("googlemaps");
            using var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Google Maps API request failed");
            return null;
        }
    }

    private bool IsOkStatus(JsonElement root, string operation, string context)
    {
        if (!root.TryGetProperty("status", out var statusElement))
            return false;

        var status = statusElement.GetString();
        if (string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "ZERO_RESULTS", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(status, "ZERO_RESULTS", StringComparison.OrdinalIgnoreCase))
                _logger.LogDebug("Google Maps {Operation} returned no results for {Context}", operation, context);
            return string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase);
        }

        _logger.LogWarning(
            "Google Maps {Operation} failed with status {Status} for {Context}",
            operation,
            status,
            context);
        return false;
    }

    private static bool TryApplyFirstGeocodeResult(
        JsonElement root,
        GoogleMapsPlaceDto result,
        bool updateCoordinates = true)
    {
        if (!root.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            return false;

        return TryApplyGeocodeEntry(results[0], result, updateCoordinates);
    }

    private static bool TryApplyGeocodeEntry(
        JsonElement entry,
        GoogleMapsPlaceDto result,
        bool updateCoordinates)
    {
        if (updateCoordinates && !TryApplyGeometry(entry, result))
            return false;

        if (entry.TryGetProperty("formatted_address", out var formattedAddress))
        {
            var address = formattedAddress.GetString()?.Trim();
            if (!string.IsNullOrEmpty(address))
                result.Address = address;
        }

        if (entry.TryGetProperty("place_id", out var placeIdElement))
        {
            var placeId = placeIdElement.GetString()?.Trim();
            if (!string.IsNullOrEmpty(placeId))
                result.PlaceId = placeId;
        }

        return updateCoordinates
            ? result.Latitude is not null && result.Longitude is not null
            : true;
    }

    private static bool TryApplyGeometry(JsonElement element, GoogleMapsPlaceDto result)
    {
        if (!element.TryGetProperty("geometry", out var geometry)
            || !geometry.TryGetProperty("location", out var location))
            return false;

        if (!TryReadCoordinate(location, "lat", out var lat)
            || !TryReadCoordinate(location, "lng", out var lng))
            return false;

        if (lat < -90 || lat > 90 || lng < -180 || lng > 180)
            return false;

        result.Latitude = lat;
        result.Longitude = lng;
        return true;
    }

    private static bool TryReadCoordinate(JsonElement location, string propertyName, out decimal value)
    {
        value = default;
        if (!location.TryGetProperty(propertyName, out var element))
            return false;

        return element.ValueKind switch
        {
            JsonValueKind.Number => element.TryGetDecimal(out value),
            JsonValueKind.String => decimal.TryParse(
                element.GetString(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value),
            _ => false
        };
    }
}
