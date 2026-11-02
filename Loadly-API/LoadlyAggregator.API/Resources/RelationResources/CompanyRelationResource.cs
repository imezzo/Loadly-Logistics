namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Company reference (e.g. in contract, trip, company document).</summary>
public class CompanyRelationResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? FullName { get; set; }
}
