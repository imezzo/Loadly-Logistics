namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;

public class AdminRevokeSessionDto
{
    public long UserId { get; set; }
    public long SessionId { get; set; }
}
