namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Location reference (e.g. trip start/end).</summary>
public class LocationRelationResource
{
    public long Id { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public string? CityNameAr { get; set; }
    public string? GovernorateName { get; set; }
    public string? GovernorateNameAr { get; set; }
    public string? DistrictName { get; set; }
    public string? DistrictNameAr { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
