namespace LoadlyAggregator.Domain.Collector.DTOs.Auth;

public class CollectorRevokeSessionDto
{
    public long UserId { get; set; }
    public long SessionId { get; set; }
}
