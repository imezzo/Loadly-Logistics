using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class NotificationChannelDisplayNames
{
    private static readonly Dictionary<NotificationChannel, (string NameEn, string NameAr)> Names = new()
    {
        { NotificationChannel.EMAIL, ("Email", "البريد الإلكتروني") },
        { NotificationChannel.SMS, ("SMS", "رسالة نصية") },
        { NotificationChannel.PUSH, ("Push", "إشعار فوري") },
        { NotificationChannel.IN_APP, ("In-App", "داخل التطبيق") }
    };

    public static string GetNameEn(NotificationChannel channel) =>
        Names.TryGetValue(channel, out var n) ? n.NameEn : channel.ToString();

    public static string GetNameAr(NotificationChannel channel) =>
        Names.TryGetValue(channel, out var n) ? n.NameAr : channel.ToString();
}
