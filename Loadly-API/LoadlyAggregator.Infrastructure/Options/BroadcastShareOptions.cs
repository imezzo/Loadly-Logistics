namespace LoadlyAggregator.Infrastructure.Options;

public class BroadcastShareOptions
{
    public const string SectionName = "BroadcastShare";

    /// <summary>
    /// Public base URL for the broadcast landing page. The broadcast's public_token is
    /// appended as the final path segment. Example: "https://app.loadly.com/broadcasts"
    /// produces "https://app.loadly.com/broadcasts/{token}". Leave empty to disable the
    /// landing URL (SMS bodies stay plain text).
    /// </summary>
    public string PublicBaseUrl { get; set; } = string.Empty;
}
