namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;



public class AdminVerifyEmailDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
