namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Lightweight sector reference (e.g. in vehicle).</summary>
public class SectorRelationResource
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
}
