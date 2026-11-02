using LoadlyAggregator.Domain.Admin.DTOs.Maps;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Maps;

public class ExtractPlaceFromGoogleMapsAction
{
    private readonly IGoogleMapsPlaceExtractor _placeExtractor;

    public ExtractPlaceFromGoogleMapsAction(IGoogleMapsPlaceExtractor placeExtractor)
    {
        _placeExtractor = placeExtractor;
    }

    public async Task<GoogleMapsPlaceDto> ExecuteAsync(ExtractPlaceFromGoogleMapsDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.GoogleMapsUrl))
            throw new ValidationException(MapsMessageKeys.GoogleMapsUrlRequired);

        var url = dto.GoogleMapsUrl.Trim();
        if (!IsValidGoogleMapsUrl(url))
            throw new ValidationException(MapsMessageKeys.InvalidGoogleMapsUrl);

        var place = await _placeExtractor.ExtractAsync(url, cancellationToken);
        place.Url = url;
        return place;
    }

    private static bool IsValidGoogleMapsUrl(string url)
    {
        return url.StartsWith("https://www.google.com/maps", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://maps.google.com", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://goo.gl/maps", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://maps.app.goo.gl", StringComparison.OrdinalIgnoreCase);
    }
}
