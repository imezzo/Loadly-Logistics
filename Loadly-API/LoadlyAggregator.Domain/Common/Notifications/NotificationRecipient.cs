using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Notifications;

/// <summary>
/// Universal message envelope produced by notification handlers.
/// The pipeline routes each recipient through preference/dedup gates and then to channel senders.
/// </summary>
public class NotificationRecipient
{
    public required long ModelId { get; init; }
    public required string ModelType { get; init; }
    public required NotificationEventType NotificationEventType { get; init; }
    public required NotificationDomain Domain { get; init; }
    public required IReadOnlySet<NotificationChannel> Channels { get; init; }

    public required string Title { get; init; }
    public required string Body { get; init; }

    public NotificationResourceType? ResourceType { get; init; }
    public long? ResourceId { get; init; }
    public string? ClickAction { get; init; }

    public string? EmailAddress { get; init; }
    public string? EmailSubject { get; init; }
    public string? EmailHtmlBody { get; init; }
    public string? PhoneNumber { get; init; }
    public Dictionary<string, string>? PushData { get; init; }
    public string? PushClickAction { get; init; }
}
