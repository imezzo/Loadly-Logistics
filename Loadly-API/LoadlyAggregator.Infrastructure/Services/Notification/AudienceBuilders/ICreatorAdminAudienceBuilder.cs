using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;


public interface ICreatorAdminAudienceBuilder
{

    Task<NotificationRecipient?> BuildAsync(
        long? addedByAdminId,
        NotificationEventType notificationEventType,
        string title,
        string body,
        Dictionary<string, string>? pushData = null,
        string? pushClickAction = null,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null);
}
