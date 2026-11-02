using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Shared.DTOs.Queue;

public class SmsQueueMessage : QueueMessageBase
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public SmsQueueMessage()
    {
        Type = QueueMessageTypes.Sms;
    }
}
