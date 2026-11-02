using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface INotificationPreferenceRepository : IRepository<Common.Entities.NotificationPreference>
{
    Task<List<Common.Entities.NotificationPreference>> GetByModelAsync(long modelId, string modelType);
    Task<Common.Entities.NotificationPreference?> GetByModelChannelAndEventAsync(long modelId, string modelType, NotificationChannel channel, NotificationEventType eventType);
    Task<bool> IsEnabledAsync(long modelId, string modelType, NotificationChannel channel, NotificationEventType eventType);
}
