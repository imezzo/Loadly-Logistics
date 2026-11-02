namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Lightweight admin reference (e.g. sales rep in contract).</summary>
public class AdminRelationResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
