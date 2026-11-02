using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminVerifyEmailRequest
{
    public string Code { get; set; } = string.Empty;

    public AdminVerifyEmailDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
