using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorDisableMfaRequest
{
    public long UserId { get; set; }

    public ContractorDisableMfaDto FromRequest(long userId) => new() { UserId = userId };
}
