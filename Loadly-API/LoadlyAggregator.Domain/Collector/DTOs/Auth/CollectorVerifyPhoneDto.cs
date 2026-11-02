namespace LoadlyAggregator.Domain.Collector.DTOs.Auth;



public class CollectorVerifyPhoneDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
