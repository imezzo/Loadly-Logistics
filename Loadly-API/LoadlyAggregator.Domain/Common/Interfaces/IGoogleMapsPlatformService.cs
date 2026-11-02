using LoadlyAggregator.Domain.Admin.DTOs.Maps;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IGoogleMapsPlatformService
{
    bool IsConfigured { get; }

    Task<bool> TryEnrichFromPlaceIdAsync(GoogleMapsPlaceDto result, CancellationToken cancellationToken = default);

    Task<bool> TryGeocodeAddressAsync(string address, GoogleMapsPlaceDto result, CancellationToken cancellationToken = default);

    Task<bool> TryReverseGeocodeAsync(GoogleMapsPlaceDto result, CancellationToken cancellationToken = default);
}
