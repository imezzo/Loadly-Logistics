using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminVerifyPhoneRequest
{
    public string Code { get; set; } = string.Empty;

    public AdminVerifyPhoneDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
