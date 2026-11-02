using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminSendMfaCodeRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Sms;

    public AdminSendMfaCodeDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}
