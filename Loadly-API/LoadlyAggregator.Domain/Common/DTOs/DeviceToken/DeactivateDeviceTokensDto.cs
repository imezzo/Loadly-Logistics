namespace LoadlyAggregator.Domain.Common.DTOs.DeviceToken;

public class DeactivateDeviceTokensDto
{
    public long UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string? FcmToken { get; set; }
    public string? DeviceId { get; set; }
    public bool RevokeAll { get; set; }
}
