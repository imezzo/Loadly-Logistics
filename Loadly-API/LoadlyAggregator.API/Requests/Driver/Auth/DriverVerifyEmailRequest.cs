using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverVerifyEmailRequest
{
    public string Code { get; set; } = string.Empty;

    public DriverVerifyEmailDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
