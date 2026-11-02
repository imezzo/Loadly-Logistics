using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Maps;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Maps;

public class CompanyExtractPlaceFromGoogleMapsAction
{
    private readonly IGoogleMapsPlaceExtractor _placeExtractor;

    public CompanyExtractPlaceFromGoogleMapsAction(IGoogleMapsPlaceExtractor placeExtractor)
    {
        _placeExtractor = placeExtractor;
    }

    public async Task<GoogleMapsPlaceDto> ExecuteAsync(ExtractPlaceFromGoogleMapsDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.GoogleMapsUrl))
        {
            throw new ValidationException(MapsMessageKeys.GoogleMapsUrlRequired);
        }

        var url = dto.GoogleMapsUrl.Trim();
        if (!IsValidGoogleMapsUrl(url))
        {
            throw new ValidationException(MapsMessageKeys.InvalidGoogleMapsUrl);
        }

        var extractedPlace = await _placeExtractor.ExtractAsync(url, cancellationToken);

        return new GoogleMapsPlaceDto
        {
            Url = url,
            Latitude = extractedPlace.Latitude,
            Longitude = extractedPlace.Longitude,
            Address = extractedPlace.Address,
            PlaceId = extractedPlace.PlaceId
        };
    }

    private static bool IsValidGoogleMapsUrl(string url)
    {
        return url.StartsWith("https://www.google.com/maps", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://maps.google.com", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://goo.gl/maps", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://maps.app.goo.gl", StringComparison.OrdinalIgnoreCase);
    }
}
