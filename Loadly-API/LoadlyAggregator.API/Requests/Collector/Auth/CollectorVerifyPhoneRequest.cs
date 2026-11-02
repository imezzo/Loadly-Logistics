using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorVerifyPhoneRequest
{
    public string Code { get; set; } = string.Empty;

    public CollectorVerifyPhoneDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
