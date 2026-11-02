namespace LoadlyAggregator.Domain.Collector.DTOs.Auth;

public class CollectorListSessionsDto
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }
}
