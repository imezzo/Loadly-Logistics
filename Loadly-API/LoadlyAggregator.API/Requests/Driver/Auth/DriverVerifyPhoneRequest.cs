using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverVerifyPhoneRequest
{
    public string Code { get; set; } = string.Empty;

    public DriverVerifyPhoneDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
