using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorEnableMfaRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    public ContractorEnableMfaDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}
