using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverLoginWithOtpRequest
{
    public string Login { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public DriverLoginWithOtpDto FromRequest(string? ipAddress = null, string? userAgent = null, string? deviceInfoJson = null, string? fcmToken = null, string? deviceId = null, string? deviceModel = null, string? deviceOs = null, string? appVersion = null) => new()
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

public class DriverLoginWithOtpRequestValidator : AbstractValidator<DriverLoginWithOtpRequest>
{
    public DriverLoginWithOtpRequestValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("validation.otp_code_required")
            .Length(6).WithMessage("validation.otp_code_six_digits");
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("validation.otp_token_required");
    }
}
