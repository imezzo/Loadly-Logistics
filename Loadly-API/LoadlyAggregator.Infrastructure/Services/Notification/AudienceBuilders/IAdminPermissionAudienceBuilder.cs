using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;


public interface IAdminPermissionAudienceBuilder
{

    Task<IReadOnlyList<NotificationRecipient>> BuildForTripEventAsync(
        string permissionName,
        string permissionGuard,
        NotificationEventType notificationEventType,
        Trip trip,
        Company company,
        string emailHtmlHeading,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null,
        CancellationToken cancellationToken = default);
}
