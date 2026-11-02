using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorVerifyEmailRequest
{
    public string Code { get; set; } = string.Empty;

    public ContractorVerifyEmailDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
