namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Vehicle facility reference (e.g. in facility visit history).</summary>
public class VehicleFacilityRelationResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public int FacilityType { get; set; }
    public long? CompanyId { get; set; }
    public CompanyRelationResource? Company { get; set; }
}
