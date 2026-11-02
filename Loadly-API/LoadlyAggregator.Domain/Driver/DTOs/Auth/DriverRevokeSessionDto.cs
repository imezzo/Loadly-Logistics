namespace LoadlyAggregator.Domain.Driver.DTOs.Auth;

public class DriverRevokeSessionDto
{
    public long UserId { get; set; }
    public long SessionId { get; set; }
}
