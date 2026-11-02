namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Contractor reference (e.g. in vehicle, driver).</summary>
public class ContractorRelationDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
}
