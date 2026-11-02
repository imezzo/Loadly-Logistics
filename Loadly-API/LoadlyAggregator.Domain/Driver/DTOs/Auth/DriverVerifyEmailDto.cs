namespace LoadlyAggregator.Domain.Driver.DTOs.Auth;



public class DriverVerifyEmailDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
}
