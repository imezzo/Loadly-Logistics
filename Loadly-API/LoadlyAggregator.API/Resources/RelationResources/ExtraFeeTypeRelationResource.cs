namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Extra fee type reference.</summary>
public class ExtraFeeTypeRelationResource
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}
