using LoadlyAggregator.Domain.Company.DTOs.Maps;

namespace LoadlyAggregator.API.Resources.Company.Maps;

public class GoogleMapsPlaceResource
{
    public string? Url { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public string? Address { get; set; }
    public string? PlaceId { get; set; }

    public static GoogleMapsPlaceResource FromDto(GoogleMapsPlaceDto dto) => new()
    {
        Url = dto.Url,
        Lat = dto.Latitude,
        Lng = dto.Longitude,
        Address = dto.Address,
        PlaceId = dto.PlaceId
    };
}
