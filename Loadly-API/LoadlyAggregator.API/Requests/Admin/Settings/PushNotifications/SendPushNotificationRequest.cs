using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Logs;

namespace LoadlyAggregator.API.Requests.Admin.Settings.PushNotifications;

public class SendPushNotificationRequest
{
    public const string Permission = "send_push_notification";

    public string DeviceToken { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string>? Data { get; set; }
    public string? ImageUrl { get; set; }
    public string? ClickAction { get; set; }
    public string Domain { get; set; } = "default";

    public SendPushNotificationDto FromRequest() => new()
    {
        DeviceToken = DeviceToken,
        Title = Title,
        Body = Body,
        Data = Data,
        ImageUrl = ImageUrl,
        ClickAction = ClickAction,
        Domain = Domain
    };
}

public class SendPushNotificationRequestValidator : AbstractValidator<SendPushNotificationRequest>
{
    public SendPushNotificationRequestValidator()
    {
        RuleFor(x => x.DeviceToken)
            .NotEmpty().WithMessage("validation.push_device_token_required")
            .MaximumLength(500).WithMessage("validation.must_not_exceed_characters|500");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("validation.push_title_required")
            .MaximumLength(100).WithMessage("validation.must_not_exceed_characters|100");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("validation.push_body_required")
            .MaximumLength(500).WithMessage("validation.must_not_exceed_characters|500");

        RuleFor(x => x.ImageUrl)
            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("validation.push_image_url_invalid")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("validation.must_not_exceed_characters|500")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.ClickAction)
            .MaximumLength(200).WithMessage("validation.must_not_exceed_characters|200")
            .When(x => !string.IsNullOrEmpty(x.ClickAction));

        RuleFor(x => x.Domain)
            .NotEmpty().WithMessage("validation.push_domain_required")
            .MaximumLength(50).WithMessage("validation.must_not_exceed_characters|50")
            .Must(domain => domain.ToLower() is "admin" or "company" or "driver" or "default")
            .WithMessage("validation.push_domain_invalid");

        RuleFor(x => x.Data)
            .Must(data => data == null || data.Count <= 20)
            .WithMessage("validation.push_data_max_items")
            .When(x => x.Data != null);

        RuleForEach(x => x.Data)
            .Must(kvp => !string.IsNullOrEmpty(kvp.Key))
            .WithMessage("validation.push_data_key_empty")
            .Must(kvp => kvp.Key.Length <= 100)
            .WithMessage("validation.push_data_key_max")
            .Must(kvp => kvp.Value.Length <= 500)
            .WithMessage("validation.push_data_value_max")
            .When(x => x.Data != null && x.Data.Any());
    }
}
