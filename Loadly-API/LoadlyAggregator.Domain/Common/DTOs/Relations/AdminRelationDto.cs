namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>
/// Lightweight admin reference for use in relation (e.g. contract sales rep). Avoids extra GET admin request.
/// </summary>
public class AdminRelationDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
