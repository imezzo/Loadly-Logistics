namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Contractor reference (e.g. in vehicle, driver).</summary>
public class ContractorRelationResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
}
