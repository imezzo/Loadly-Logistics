using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyLoginWithOtpRequest
{
    public string Login { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public CompanyLoginWithOtpDto FromRequest(string? ipAddress = null, string? userAgent = null, string? deviceInfoJson = null, string? fcmToken = null, string? deviceId = null, string? deviceModel = null, string? deviceOs = null, string? appVersion = null) => new()
    {
        Login = Login,
        Code = Code,
        Token = Token,
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

public class CompanyLoginWithOtpRequestValidator : AbstractValidator<CompanyLoginWithOtpRequest>
{
    public CompanyLoginWithOtpRequestValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required")
            .Length(6).WithMessage("OTP code must be 6 digits");
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");
    }
}
