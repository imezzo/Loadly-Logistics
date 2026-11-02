namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Vehicle brand reference.</summary>
public class BrandRelationResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}
