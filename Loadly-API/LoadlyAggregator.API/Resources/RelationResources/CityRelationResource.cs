namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Lightweight city reference in address.</summary>
public class CityRelationResource
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
}
