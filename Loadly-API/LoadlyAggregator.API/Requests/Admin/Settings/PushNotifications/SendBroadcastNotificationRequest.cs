using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.PushNotifications;

public class SendBroadcastNotificationRequest
{
    public const string Permission = "send_broadcast_notification";

    public const int MaxAttachments = 5;
    public const long MaxAttachmentSizeBytes = 10 * 1024 * 1024; // 10 MB
    public static readonly string[] AllowedAttachmentTypes =
        { "pdf", "png", "jpg", "jpeg", "gif", "mp4" };

    public BroadcastChannel Channel { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public bool SendToAllUsers { get; set; }
    public List<long> ModelIds { get; set; } = [];
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? ClickAction { get; set; }
    public List<FileUpload>? Attachments { get; set; }

    public SendBroadcastNotificationDto FromRequest() => new()
    {
        Channel = Channel,
        ModelType = ModelType,
        ModelIds = SendToAllUsers ? [] : ModelIds,
        Subject = Subject,
        Body = Body,
        ClickAction = ClickAction,
        Attachments = Attachments
    };
}

public class SendBroadcastNotificationRequestValidator : AbstractValidator<SendBroadcastNotificationRequest>
{
    public SendBroadcastNotificationRequestValidator()
    {
        RuleFor(x => x.Channel)
            .IsInEnum().WithMessage("Invalid broadcast channel");

        RuleFor(x => x.ModelType)
            .NotEmpty().WithMessage("Model type is required")
            .Must(t => t.ToLowerInvariant() is "admin" or "company" or "driver" or "collector" or "contractor")
            .WithMessage("Model type must be one of: admin, company, driver, collector, contractor");

        RuleFor(x => x.ModelIds)
            .Empty()
            .When(x => x.SendToAllUsers)
            .WithMessage("notifications.model_ids_must_be_empty_when_send_to_all");

        RuleFor(x => x.ModelIds)
            .NotEmpty()
            .When(x => !x.SendToAllUsers)
            .WithMessage("notifications.model_ids_required");

        RuleFor(x => x.ModelIds)
            .Must(ids => ids.All(id => id > 0))
            .When(x => !x.SendToAllUsers)
            .WithMessage("notifications.model_ids_must_be_positive");

        RuleFor(x => x.Subject)
            .MaximumLength(500).WithMessage("Subject must not exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Subject));

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Message body is required")
            .MaximumLength(5000).WithMessage("Body must not exceed 5000 characters");

        RuleFor(x => x.Attachments)
            .Must(files => files == null || files.Count <= SendBroadcastNotificationRequest.MaxAttachments)
            .WithMessage($"notifications.too_many_attachments|{SendBroadcastNotificationRequest.MaxAttachments}")
            .When(x => x.Attachments != null);

        RuleForEach(x => x.Attachments)
            .Must(file => file.Length <= SendBroadcastNotificationRequest.MaxAttachmentSizeBytes)
            .WithMessage("notifications.attachment_too_large|10")
            .Must(file => SendBroadcastNotificationRequest.AllowedAttachmentTypes
                .Any(type => file.FileName.EndsWith($".{type}", StringComparison.OrdinalIgnoreCase)
                             || file.ContentType.Contains(type, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("notifications.attachment_type_not_allowed")
            .When(x => x.Attachments != null);
    }
}
