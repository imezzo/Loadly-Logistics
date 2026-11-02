using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyVerifyEmailRequest
{
    public string Code { get; set; } = string.Empty;

    public CompanyVerifyEmailDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
