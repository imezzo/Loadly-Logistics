using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorResendVerificationRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    public CollectorResendVerificationDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}
