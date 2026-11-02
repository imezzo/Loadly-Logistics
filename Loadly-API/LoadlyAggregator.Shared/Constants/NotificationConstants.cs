namespace LoadlyAggregator.Shared.Constants;

public static class QueueProcessingDefaults
{
    public const int BroadcastNotificationBatchSize = 200;
}


public static class QueueMessageTypes
{
    public const string Email = "email";
    public const string Sms = "sms";
    public const string PushNotification = "push_notification";
    public const string BroadcastNotification = "broadcast_notification";
}
