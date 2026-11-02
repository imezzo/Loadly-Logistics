namespace LoadlyAggregator.Domain.Collector.DTOs.Auth;



public class CollectorVerifyEmailDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
