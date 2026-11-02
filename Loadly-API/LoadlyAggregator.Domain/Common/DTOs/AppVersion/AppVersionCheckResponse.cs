namespace LoadlyAggregator.Domain.Common.DTOs.AppVersion;

public class AppVersionCheckResponse
{
    public string ClientVersion { get; set; } = string.Empty;
    public string MinVersion { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public bool ForceUpdate { get; set; }
    public bool SoftUpdate { get; set; }
    public string? StoreUrl { get; set; }
}
