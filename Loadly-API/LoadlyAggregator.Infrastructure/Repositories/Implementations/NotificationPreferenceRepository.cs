using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class NotificationPreferenceRepository : Repository<NotificationPreference>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<NotificationPreference>> GetByModelAsync(long modelId, string modelType)
    {
        return await _dbSet
            .Where(p => p.ModelId == modelId && p.ModelType == modelType)
            .ToListAsync();
    }

    public async Task<NotificationPreference?> GetByModelChannelAndEventAsync(long modelId, string modelType, NotificationChannel channel, NotificationEventType eventType)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.ModelId == modelId
                && p.ModelType == modelType
                && p.Channel == channel
                && p.EventType == eventType);
    }

    public async Task<bool> IsEnabledAsync(long modelId, string modelType, NotificationChannel channel, NotificationEventType eventType)
    {
        var preference = await GetByModelChannelAndEventAsync(modelId, modelType, channel, eventType);
        return preference?.IsEnabled ?? true;
    }
}
