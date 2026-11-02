using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;

namespace LoadlyAggregator.API.Requests.Driver.DeviceToken;

public class RegisterDeviceTokenRequest
{
    public string FcmToken { get; set; } = string.Empty;

    public RegisterDeviceTokenDto FromRequest(long userId, string? deviceId, string? deviceModel, string? deviceOs, string? appVersion) => new()
    {
        UserId = userId,
        UserType = "driver",
        FcmToken = FcmToken,
        DeviceId = deviceId,
        DeviceModel = deviceModel,
        DeviceOs = deviceOs,
        AppVersion = appVersion
    };
}

public class RegisterDeviceTokenRequestValidator : AbstractValidator<RegisterDeviceTokenRequest>
{
    public RegisterDeviceTokenRequestValidator()
    {
        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("validation.fcm_token_required")
            .MaximumLength(500).WithMessage("validation.must_not_exceed_characters|500");
    }
}
