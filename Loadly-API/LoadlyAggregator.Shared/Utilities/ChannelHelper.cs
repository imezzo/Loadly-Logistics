using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Shared.Utilities;



public static class ChannelHelper
{
    public static CommunicationChannel ParseChannel(string? channel)
    {
        if (string.IsNullOrWhiteSpace(channel))
            return CommunicationChannel.Email;
        return channel.ToLower() switch
        {
            "email" => CommunicationChannel.Email,
            "sms" => CommunicationChannel.Sms,
            _ => CommunicationChannel.Email
        };
    }
    public static string ToChannelString(CommunicationChannel channel)
    {
        return channel switch
        {
            CommunicationChannel.Email => "email",
            CommunicationChannel.Sms => "sms",
            _ => "email"
        };
    }
}
