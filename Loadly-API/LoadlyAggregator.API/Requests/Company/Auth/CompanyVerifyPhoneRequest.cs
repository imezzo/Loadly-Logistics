using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyVerifyPhoneRequest
{
    public string Code { get; set; } = string.Empty;

    public CompanyVerifyPhoneDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
