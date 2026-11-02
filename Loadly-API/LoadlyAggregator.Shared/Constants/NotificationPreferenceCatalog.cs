using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Shared.Constants;

/// <summary>
/// User preference matrix for notifications. Rows are derived from <see cref="NotificationDefinitionRegistry"/>; do not duplicate rules here.
/// </summary>
public static class NotificationPreferenceCatalog
{
    public sealed record Entry(NotificationEventType EventType, NotificationChannel Channel);

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<Entry>> ByModelType =
        NotificationDefinitionRegistry.BuildPreferenceCatalogByModelType();

    public static string NormalizeModelType(string? userTypeClaim) =>
        userTypeClaim?.ToLowerInvariant() switch
        {
            "admin" => "Admin",
            "company" => "Company",
            "driver" => "Driver",
            "contractor" => "Contractor",
            "collector" => "Collector",
            _ => ""
        };

    public static IReadOnlyList<Entry> GetEntriesForModelType(string normalizedModelType)
    {
        if (ByModelType.TryGetValue(normalizedModelType, out IReadOnlyList<Entry>? entries))
            return entries;
        return Array.Empty<Entry>();
    }

    public static bool IsConfigurable(string normalizedModelType, NotificationChannel channel, NotificationEventType eventType) =>
        NotificationDefinitionRegistry.IsPreferenceConfigurable(normalizedModelType, channel, eventType);
}
