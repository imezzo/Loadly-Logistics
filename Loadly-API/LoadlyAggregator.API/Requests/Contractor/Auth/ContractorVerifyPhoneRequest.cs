using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorVerifyPhoneRequest
{
    public string Code { get; set; } = string.Empty;

    public ContractorVerifyPhoneDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
