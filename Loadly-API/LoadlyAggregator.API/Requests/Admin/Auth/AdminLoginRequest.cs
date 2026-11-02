using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminLoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public AdminLoginDto FromRequest(string? ipAddress = null, string? userAgent = null, string? deviceInfoJson = null, string? fcmToken = null, string? deviceId = null, string? deviceModel = null, string? deviceOs = null, string? appVersion = null) => new()
    {
        Login = Login,
        Password = Password,
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

public class AdminLoginRequestValidator : AbstractValidator<AdminLoginRequest>
{
    public AdminLoginRequestValidator()
    {
        RuleFor(x => x.Login).MustBeAdminLoginIdentifier();
        RuleFor(x => x.Password).IsValidPassword();
    }
}
