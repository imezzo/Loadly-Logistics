namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

public class LeadRelationDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}
