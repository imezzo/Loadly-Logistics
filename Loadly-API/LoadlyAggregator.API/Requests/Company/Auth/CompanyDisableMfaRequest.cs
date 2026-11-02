using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyDisableMfaRequest
{
    public long UserId { get; set; }

    public CompanyDisableMfaDto FromRequest(long userId) => new() { UserId = userId };
}
