namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Vehicle model reference (Id, names, Brand).</summary>
public class ModelRelationResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public BrandRelationResource? Brand { get; set; }
}
