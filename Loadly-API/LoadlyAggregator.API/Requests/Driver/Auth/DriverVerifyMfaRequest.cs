using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverVerifyMfaRequest
{
    public string Code { get; set; } = string.Empty;

    public DriverVerifyMfaDto FromRequest(long userId, string userType, string? ipAddress = null, string? userAgent = null, string? deviceInfoJson = null, string? fcmToken = null, string? deviceId = null, string? deviceModel = null, string? deviceOs = null, string? appVersion = null) => new()
    {
        Code = Code,
        UserId = userId,
        UserType = userType,
        IpAddress = ipAddress,
        UserAgent = userAgent,
        DeviceInfoJson = deviceInfoJson,
        FcmToken = fcmToken,
        DeviceId = deviceId,
        DeviceModel = deviceModel,
        DeviceOs = deviceOs,
        AppVersion = appVersion
    };
}
