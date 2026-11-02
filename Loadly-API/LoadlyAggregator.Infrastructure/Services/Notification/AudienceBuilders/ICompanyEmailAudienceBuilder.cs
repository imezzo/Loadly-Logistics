using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;

/// <summary>
/// Builds a company <see cref="NotificationRecipient"/> for trip-related events.
/// </summary>
public interface ICompanyEmailAudienceBuilder
{

    Task<NotificationRecipient?> BuildAsync(
        long companyId,
        long tripId,
        NotificationEventType notificationEventType,
        NotificationChannel[]? additionalChannels = null,
        string? titleOverride = null,
        string? bodyOverride = null,
        Dictionary<string, string>? pushData = null,
        string? pushClickAction = null,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null);
}
