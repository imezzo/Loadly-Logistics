using FluentValidation;
using LoadlyAggregator.Domain.Common.DTOs.AppVersion;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Common.AppVersion;

public class CheckAppVersionRequest
{
    public string Platform { get; set; } = string.Empty;
    public string AppType { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public CheckAppVersionDto FromRequest() => new()
    {
        Platform = Enum.Parse<AppPlatform>(Platform, ignoreCase: true),
        AppType = Enum.Parse<AppType>(AppType, ignoreCase: true),
        ClientVersion = Version.Trim()
    };
}

public class CheckAppVersionRequestValidator : AbstractValidator<CheckAppVersionRequest>
{
    public CheckAppVersionRequestValidator()
    {
        RuleFor(x => x.Platform)
            .NotEmpty().WithMessage("validation.required")
            .Must(BeValidPlatform).WithMessage("app_version.invalid_platform");

        RuleFor(x => x.AppType)
            .NotEmpty().WithMessage("validation.required")
            .Must(BeValidAppType).WithMessage("app_version.invalid_app_type");

        RuleFor(x => x.Version)
            .NotEmpty().WithMessage("validation.required")
            .MaximumLength(32).WithMessage("validation.must_not_exceed_characters|32")
            .Must(BeValidVersion).WithMessage("app_version.invalid_format");
    }

    private static bool BeValidPlatform(string value) =>
        Enum.TryParse<AppPlatform>(value, ignoreCase: true, out _);

    private static bool BeValidAppType(string value) =>
        Enum.TryParse<AppType>(value, ignoreCase: true, out _);

    private static bool BeValidVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }
        var normalized = value.Split('-', '+')[0].Trim();
        return System.Version.TryParse(normalized, out _);
    }
}
