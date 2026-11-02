namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Contract reference (e.g. in trip).</summary>
public class ContractRelationResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public int ContractType { get; set; }
    public string ContractTypeName { get; set; } = string.Empty;
    public int ConsumptionType { get; set; }
    public CompanyRelationResource? Company { get; set; }
}
