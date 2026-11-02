using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class NotificationEventTypeDisplayNames
{
    public static string GetNameEn(NotificationEventType eventType)
    {
        var def = NotificationDefinitionRegistry.TryGetDefinition(eventType);
        return def?.DisplayNameEn ?? eventType.ToString();
    }

    public static string GetNameAr(NotificationEventType eventType)
    {
        var def = NotificationDefinitionRegistry.TryGetDefinition(eventType);
        return def?.DisplayNameAr ?? eventType.ToString();
    }
}
