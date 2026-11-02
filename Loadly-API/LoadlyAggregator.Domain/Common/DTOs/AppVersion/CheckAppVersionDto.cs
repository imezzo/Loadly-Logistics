using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.AppVersion;

public class CheckAppVersionDto
{
    public AppPlatform Platform { get; set; }
    public AppType AppType { get; set; }
    public string ClientVersion { get; set; } = string.Empty;
}
