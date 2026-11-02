using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorVerifyEmailRequest
{
    public string Code { get; set; } = string.Empty;

    public CollectorVerifyEmailDto FromRequest(long userId) => new() { Code = Code, UserId = userId };
}
