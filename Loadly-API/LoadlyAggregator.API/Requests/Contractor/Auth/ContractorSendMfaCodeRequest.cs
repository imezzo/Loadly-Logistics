using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorSendMfaCodeRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;

    public ContractorSendMfaCodeDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}
