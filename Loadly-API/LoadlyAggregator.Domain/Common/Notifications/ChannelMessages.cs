using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Notifications;

public sealed record EmailMessage(
    string To,
    string ToName,
    string Subject,
    string Body,
    bool IsHtml,
    NotificationDomain Domain,
    NotificationEventType? EventType = null,
    string? From = null,
    string? FromName = null);

public sealed record SmsMessage(
    string PhoneNumber,
    string Message,
    NotificationDomain Domain,
    NotificationEventType? EventType = null);

public sealed record PushUserMessage(
    long ModelId,
    string ModelType,
    string Title,
    string Body,
    NotificationDomain Domain,
    NotificationEventType? EventType = null,
    Dictionary<string, string>? Data = null,
    string? ClickAction = null);

public sealed record PushDeviceMessage(
    string DeviceToken,
    string Title,
    string Body,
    NotificationDomain Domain,
    NotificationEventType? EventType = null,
    Dictionary<string, string>? Data = null,
    string? ImageUrl = null,
    string? ClickAction = null);
