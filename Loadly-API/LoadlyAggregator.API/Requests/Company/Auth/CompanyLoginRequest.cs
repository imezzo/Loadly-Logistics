using FluentValidation;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyLoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public CompanyLoginDto FromRequest(string? ipAddress = null, string? userAgent = null, string? deviceInfoJson = null, string? fcmToken = null, string? deviceId = null, string? deviceModel = null, string? deviceOs = null, string? appVersion = null) => new()
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

public class CompanyLoginRequestValidator : AbstractValidator<CompanyLoginRequest>
{
    public CompanyLoginRequestValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
    }
}
