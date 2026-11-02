using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Maps;

namespace LoadlyAggregator.Domain.Company.Actions.Maps;

public class CompanyExtractPlacesFromGoogleMapsAction
{
    private readonly IGoogleMapsPlaceExtractor _placeExtractor;

    public CompanyExtractPlacesFromGoogleMapsAction(IGoogleMapsPlaceExtractor placeExtractor)
    {
        _placeExtractor = placeExtractor;
    }

    public async Task<List<GoogleMapsPlaceDto>> ExecuteAsync(
        ExtractPlacesFromGoogleMapsDto dto,
        CancellationToken cancellationToken = default)
    {
        var urls = dto.GoogleMapsUrls
            .Select(u => u.Trim())
            .ToList();

        var extractPlaceTasks = urls.Select(async url =>
        {
            var extractedPlace = await _placeExtractor.ExtractAsync(url, cancellationToken);
            return new GoogleMapsPlaceDto
            {
                Url = url,
                Latitude = extractedPlace.Latitude,
                Longitude = extractedPlace.Longitude,
                Address = extractedPlace.Address,
                PlaceId = extractedPlace.PlaceId
            };
        });

        var results = await Task.WhenAll(extractPlaceTasks);
        return results.ToList();
    }
}

