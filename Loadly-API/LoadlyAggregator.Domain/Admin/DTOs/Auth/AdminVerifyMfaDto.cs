namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;

public class AdminVerifyMfaDto
{
    public string Code { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string UserType { get; set; } = "admin";
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceInfoJson { get; set; }
    public string? FcmToken { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceModel { get; set; }
    public string? DeviceOs { get; set; }
    public string? AppVersion { get; set; }
}
