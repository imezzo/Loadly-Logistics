using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminResendVerificationRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    public AdminResendVerificationDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}
