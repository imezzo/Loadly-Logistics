using System.Text.Json;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Infrastructure.Helpers;

public static class DeviceInfoHelper
{
    public static string? GetIpAddress(HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var ip = forwardedFor.Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(ip))
            {
                return ip;
            }
        }

        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return realIp;
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    public static string? GetUserAgent(HttpContext httpContext)
    {
        return httpContext.Request.Headers["User-Agent"].FirstOrDefault();
    }

    public static (string? IpAddress, string? UserAgent) GetDeviceInfo(HttpContext httpContext)
    {
        return (GetIpAddress(httpContext), GetUserAgent(httpContext));
    }

    public static DeviceInfo GetCompleteDeviceInfo(HttpContext httpContext)
    {
        return new DeviceInfo
        {
            DeviceId = httpContext.Request.Headers["device-id"].FirstOrDefault(),
            DeviceModel = httpContext.Request.Headers["device-model"].FirstOrDefault(),
            DeviceOs = httpContext.Request.Headers["device-os"].FirstOrDefault(),
            DeviceOsVersion = httpContext.Request.Headers["device-os-version"].FirstOrDefault(),
            AppVersion = httpContext.Request.Headers["app-version"].FirstOrDefault(),
            AppBuildNumber = httpContext.Request.Headers["app-build-number"].FirstOrDefault(),
            IpAddress = GetIpAddress(httpContext),
            UserAgent = GetUserAgent(httpContext)
        };
    }

    public static string? GetDeviceInfoJson(HttpContext httpContext)
    {
        var deviceInfo = GetCompleteDeviceInfo(httpContext);
        var hasAnyDeviceInfo = !string.IsNullOrWhiteSpace(deviceInfo.DeviceId) ||
                               !string.IsNullOrWhiteSpace(deviceInfo.DeviceModel) ||
                               !string.IsNullOrWhiteSpace(deviceInfo.DeviceOs) ||
                               !string.IsNullOrWhiteSpace(deviceInfo.DeviceOsVersion) ||
                               !string.IsNullOrWhiteSpace(deviceInfo.AppVersion) ||
                               !string.IsNullOrWhiteSpace(deviceInfo.AppBuildNumber);

        return hasAnyDeviceInfo ? JsonSerializer.Serialize(deviceInfo) : null;
    }

    public static string? GetFcmToken(HttpContext httpContext)
    {
        return httpContext.Request.Headers["fcmtoken"].FirstOrDefault()
            ?? httpContext.Request.Headers["fcm-token"].FirstOrDefault();
    }
}
