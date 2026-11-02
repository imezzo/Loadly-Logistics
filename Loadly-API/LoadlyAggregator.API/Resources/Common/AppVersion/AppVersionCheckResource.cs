using LoadlyAggregator.Domain.Common.DTOs.AppVersion;

namespace LoadlyAggregator.API.Resources.Common.AppVersion;

public class AppVersionCheckResource
{
    public string ClientVersion { get; set; } = string.Empty;
    public string MinVersion { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public bool ForceUpdate { get; set; }
    public bool SoftUpdate { get; set; }
    public string? StoreUrl { get; set; }

    public static AppVersionCheckResource FromResponse(AppVersionCheckResponse response) => new()
    {
        ClientVersion = response.ClientVersion,
        MinVersion = response.MinVersion,
        LatestVersion = response.LatestVersion,
        ForceUpdate = response.ForceUpdate,
        SoftUpdate = response.SoftUpdate,
        StoreUrl = response.StoreUrl
    };
}
