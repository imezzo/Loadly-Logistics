namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Driver reference (e.g. in trip, vehicle).</summary>
public class DriverRelationResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public string? FullNameEn { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? NationalId { get; set; }
}
