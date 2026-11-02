using LoadlyAggregator.Domain.Admin.DTOs.Maps;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IGoogleMapsPlaceExtractor
{
    Task<GoogleMapsPlaceDto> ExtractAsync(string googleMapsUrl, CancellationToken cancellationToken = default);
}
