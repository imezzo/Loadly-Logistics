namespace LoadlyAggregator.API.Resources.RelationResources;

public class LeadRelationResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}
