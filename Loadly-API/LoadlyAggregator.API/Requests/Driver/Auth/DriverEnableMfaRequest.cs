using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverEnableMfaRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    public DriverEnableMfaDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}
