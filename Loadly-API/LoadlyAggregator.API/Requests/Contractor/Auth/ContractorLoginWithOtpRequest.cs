using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorLoginWithOtpRequest
{
    public string Login { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public ContractorLoginWithOtpDto FromRequest(string? ipAddress = null, string? userAgent = null, string? deviceInfoJson = null, string? fcmToken = null, string? deviceId = null, string? deviceModel = null, string? deviceOs = null, string? appVersion = null) => new()
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

public class ContractorLoginWithOtpRequestValidator : AbstractValidator<ContractorLoginWithOtpRequest>
{
    public ContractorLoginWithOtpRequestValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required")
            .Length(6).WithMessage("OTP code must be 6 digits");
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");
    }
}
