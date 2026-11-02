using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;

/// <summary>
/// Builds <see cref="NotificationRecipient"/>s for every active user that belongs to a company.
/// </summary>
public interface ICompanyUsersAudienceBuilder
{
    Task<IReadOnlyList<NotificationRecipient>> BuildAsync(
        long companyId,
        NotificationEventType notificationEventType,
        string title,
        string body,
        HashSet<NotificationChannel>? channels = null,
        Dictionary<string, string>? pushData = null,
        string? pushClickAction = null,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null);
}
