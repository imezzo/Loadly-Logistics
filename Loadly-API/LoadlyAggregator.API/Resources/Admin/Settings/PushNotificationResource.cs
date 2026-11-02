namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class PushNotificationResource
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;

    public static PushNotificationResource FromResult(bool success, string deviceToken, string title, string domain)
    {
        return new PushNotificationResource
        {
            Success = success,
            Message = success ? "push_notifications.sent_successfully" : "push_notifications.send_failed",
            DeviceToken = deviceToken,
            Title = title,
            Domain = domain
        };
    }
}
