using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Admin.DTOs.Maps;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Services.Maps;


public class GoogleMapsPlaceExtractor : IGoogleMapsPlaceExtractor
{
    private readonly IGoogleMapsPlatformService _googleMapsPlatformService;
    private readonly ILogger<GoogleMapsPlaceExtractor> _logger;

    public GoogleMapsPlaceExtractor(
        IGoogleMapsPlatformService googleMapsPlatformService,
        ILogger<GoogleMapsPlaceExtractor> logger)
    {
        _googleMapsPlatformService = googleMapsPlatformService;
        _logger = logger;
    }

    public async Task<GoogleMapsPlaceDto> ExtractAsync(string googleMapsUrl, CancellationToken cancellationToken = default)
    {
        var result = new GoogleMapsPlaceDto();
        var decodedUrl = Uri.UnescapeDataString(googleMapsUrl);

        GoogleMapsUrlParser.ExtractFromUrl(decodedUrl, result);

        if (GoogleMapsUrlParser.IsShortUrl(decodedUrl))
            await ResolveShortUrlAndExtractAsync(decodedUrl, result, cancellationToken);

        await TryEnrichFromGooglePlaceIdAsync(result, cancellationToken);
        await EnsureCoordinatesAsync(result, cancellationToken);
        await EnsureAddressAsync(result, cancellationToken);

        return result;
    }

    private async Task ResolveShortUrlAndExtractAsync(string shortUrl, GoogleMapsPlaceDto result, CancellationToken cancellationToken)
    {
        try
        {
            var finalUrl = await ShortUrlResolver.ResolveAsync(shortUrl, isMobile: false, cancellationToken);
            if (!string.IsNullOrEmpty(finalUrl))
                GoogleMapsUrlParser.ExtractFromUrl(Uri.UnescapeDataString(finalUrl), result);

            if (result.Latitude is null || result.Longitude is null)
            {
                finalUrl = await ShortUrlResolver.ResolveAsync(shortUrl, isMobile: true, cancellationToken);
                if (!string.IsNullOrEmpty(finalUrl))
                    GoogleMapsUrlParser.ExtractFromUrl(Uri.UnescapeDataString(finalUrl), result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve short URL {Url}: {Message}", shortUrl, ex.Message);
        }
    }

    private async Task TryEnrichFromGooglePlaceIdAsync(GoogleMapsPlaceDto result, CancellationToken cancellationToken)
    {
        if (!_googleMapsPlatformService.IsConfigured
            || !GoogleMapsPlaceIdHelper.IsResolvableWithPlaceDetailsApi(result.PlaceId))
        {
            return;
        }

        try
        {
            await _googleMapsPlatformService.TryEnrichFromPlaceIdAsync(result, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Google Place Details failed for place id {PlaceId}", result.PlaceId);
        }
    }

    private async Task EnsureCoordinatesAsync(GoogleMapsPlaceDto result, CancellationToken cancellationToken)
    {
        if (result.Latitude is not null && result.Longitude is not null)
            return;

        if (!string.IsNullOrWhiteSpace(result.Address))
        {
            try
            {
                await TryGeocodeAddressAsync(result.Address, result, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Geocoding failed for address {Address}", result.Address);
            }
        }

        if (result.Latitude is null || result.Longitude is null)
            throw new ValidationException(MapsMessageKeys.ExtractFailed);
    }

    private async Task EnsureAddressAsync(GoogleMapsPlaceDto result, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(result.Address))
            return;

        if (result.Latitude is null || result.Longitude is null)
            return;

        try
        {
            if (_googleMapsPlatformService.IsConfigured
                && await _googleMapsPlatformService.TryReverseGeocodeAsync(result, cancellationToken))
            {
                return;
            }

            await GeocodingService.TryReverseGeocodeAsync(result, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Reverse geocoding failed for {Lat},{Lng}", result.Latitude, result.Longitude);
        }
    }

    private async Task TryGeocodeAddressAsync(
        string address,
        GoogleMapsPlaceDto result,
        CancellationToken cancellationToken)
    {
        var queries = new[] { address }
            .Concat(AddressFallbackQueryBuilder.BuildFallbackQueries(address))
            .Where(q => !string.IsNullOrWhiteSpace(q))
            .Distinct()
            .ToList();

        if (_googleMapsPlatformService.IsConfigured)
        {
            foreach (var query in queries)
            {
                if (await _googleMapsPlatformService.TryGeocodeAddressAsync(query, result, cancellationToken))
                    return;
            }
        }

        await GeocodingService.TryGeocodeAsync(address, result, cancellationToken);
    }
}
