using LoadlyAggregator.Domain.Admin.DTOs.Maps;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Maps;

public class ExtractPlacesFromGoogleMapsAction
{
    private readonly IGoogleMapsPlaceExtractor _placeExtractor;

    public ExtractPlacesFromGoogleMapsAction(IGoogleMapsPlaceExtractor placeExtractor)
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
            var place = await _placeExtractor.ExtractAsync(url, cancellationToken);
            place.Url = url;
            return place;
        });

        var results = await Task.WhenAll(extractPlaceTasks);
        return results.ToList();
    }
}


