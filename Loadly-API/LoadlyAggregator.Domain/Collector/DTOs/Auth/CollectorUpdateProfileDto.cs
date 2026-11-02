namespace LoadlyAggregator.Domain.Collector.DTOs.Auth;



public class CollectorUpdateProfileDto
{
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
}
