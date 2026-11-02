namespace LoadlyAggregator.Domain.Common.DTOs.DeviceToken;

public class RegisterDeviceTokenDto
{
    public long UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string FcmToken { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
    public string? DeviceModel { get; set; }
    public string? DeviceOs { get; set; }
    public string? AppVersion { get; set; }
}
