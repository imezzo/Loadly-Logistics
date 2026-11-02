namespace LoadlyAggregator.Shared.DTOs.Common;

public class DeviceInfo
{
    public string? DeviceId { get; set; }
    public string? DeviceModel { get; set; }
    public string? DeviceOs { get; set; }
    public string? DeviceOsVersion { get; set; }
    public string? AppVersion { get; set; }
    public string? AppBuildNumber { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
