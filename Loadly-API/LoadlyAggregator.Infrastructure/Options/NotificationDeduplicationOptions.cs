namespace LoadlyAggregator.Infrastructure.Options;

public class NotificationDeduplicationOptions
{
    public const string SectionName = "NotificationDeduplication";

    /// <summary>
    /// Whether deduplication is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Time window in minutes. Duplicate notifications within this window are suppressed. Default: 5.
    /// </summary>
    public int WindowMinutes { get; set; } = 5;

    /// <summary>
    /// Redis key prefix for deduplication entries. Default: "notif_dedup".
    /// </summary>
    public string KeyPrefix { get; set; } = "notif_dedup";
}
