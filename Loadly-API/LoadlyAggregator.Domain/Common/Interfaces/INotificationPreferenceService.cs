using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface INotificationPreferenceService
{
    Task<bool> ShouldSendAsync(long modelId, string modelType, NotificationChannel channel, NotificationEventType eventType);
    Task<IReadOnlyList<NotificationPreferenceItemDto>> GetEffectivePreferencesAsync(long userId, string? userType);
    Task SetNotificationPreferenceAsync(long userId, string? userType, NotificationChannel channel, NotificationEventType eventType, bool isEnabled);
}
