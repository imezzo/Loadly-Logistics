using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class NotificationPreferenceService : INotificationPreferenceService
{
    private readonly INotificationPreferenceRepository _preferenceRepository;

    public NotificationPreferenceService(INotificationPreferenceRepository preferenceRepository)
    {
        _preferenceRepository = preferenceRepository;
    }

    public Task<bool> ShouldSendAsync(long modelId, string modelType, NotificationChannel channel, NotificationEventType eventType) =>
        _preferenceRepository.IsEnabledAsync(modelId, modelType, channel, eventType);

    public async Task<IReadOnlyList<NotificationPreferenceItemDto>> GetEffectivePreferencesAsync(long userId, string? userType)
    {
        string modelType = ResolveModelTypeFromPrincipalOrThrow(userType);
        IReadOnlyList<NotificationPreferenceCatalog.Entry> catalog = NotificationPreferenceCatalog.GetEntriesForModelType(modelType);
        List<NotificationPreference> stored = await _preferenceRepository.GetByModelAsync(userId, modelType);
        return MergeCatalogWithStoredRows(catalog, stored);
    }

    public async Task SetNotificationPreferenceAsync(
        long userId,
        string? userType,
        NotificationChannel channel,
        NotificationEventType eventType,
        bool isEnabled)
    {
        string modelType = ResolveModelTypeFromPrincipalOrThrow(userType);

        if (!NotificationPreferenceCatalog.IsConfigurable(modelType, channel, eventType))
            throw new ValidationException("notifications.preference_not_configurable");

        NotificationPreference? existing = await _preferenceRepository.GetByModelChannelAndEventAsync(userId, modelType, channel, eventType);

        if (isEnabled)
            RemoveOverrideIfPresent(existing);
        else
            await EnsureDisabledRowAsync(userId, modelType, channel, eventType, existing);

        await _preferenceRepository.SaveChangesAsync();
    }

    private static string ResolveModelTypeFromPrincipalOrThrow(string? userType)
    {
        string normalized = NotificationPreferenceCatalog.NormalizeModelType(userType);
        if (string.IsNullOrEmpty(normalized))
            throw new ForbiddenException("notifications.preference_model_unknown");
        return normalized;
    }

    private static IReadOnlyList<NotificationPreferenceItemDto> MergeCatalogWithStoredRows(
        IReadOnlyList<NotificationPreferenceCatalog.Entry> catalog,
        List<NotificationPreference> storedRows)
    {
        var storedByKey = storedRows.ToDictionary(
            p => (p.Channel, p.EventType),
            p => p.IsEnabled);

        return catalog
            .Select(entry => new NotificationPreferenceItemDto
            {
                EventType = entry.EventType,
                Channel = entry.Channel,
                IsEnabled = storedByKey.TryGetValue((entry.Channel, entry.EventType), out bool enabled)
                    ? enabled
                    : true
            })
            .ToList();
    }

    private void RemoveOverrideIfPresent(NotificationPreference? existing)
    {
        if (existing != null)
            _preferenceRepository.Remove(existing);
    }

    private async Task EnsureDisabledRowAsync(
        long userId,
        string modelType,
        NotificationChannel channel,
        NotificationEventType eventType,
        NotificationPreference? existing)
    {
        if (existing == null)
        {
            await _preferenceRepository.AddAsync(new NotificationPreference
            {
                ModelId = userId,
                ModelType = modelType,
                Channel = channel,
                EventType = eventType,
                IsEnabled = false
            });
            return;
        }

        existing.IsEnabled = false;
        _preferenceRepository.Update(existing);
    }
}
