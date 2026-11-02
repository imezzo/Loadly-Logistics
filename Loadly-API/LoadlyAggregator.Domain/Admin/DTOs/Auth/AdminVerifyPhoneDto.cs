namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;



public class AdminVerifyPhoneDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
