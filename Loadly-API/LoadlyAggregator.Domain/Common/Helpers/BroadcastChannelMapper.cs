using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;


public static class BroadcastChannelMapper
{

    public static NotificationChannel ToNotificationChannel(this BroadcastChannel channel)
    {
        return channel switch
        {
            BroadcastChannel.EMAIL => NotificationChannel.EMAIL,
            BroadcastChannel.SMS => NotificationChannel.SMS,
            BroadcastChannel.IN_APP => NotificationChannel.PUSH,  // IN_APP means: push to devices + store in DB
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, "Unknown broadcast channel")
        };
    }

    public static bool RequiresInAppStorage(this BroadcastChannel channel)
    {
        return channel == BroadcastChannel.IN_APP;
    }
}
