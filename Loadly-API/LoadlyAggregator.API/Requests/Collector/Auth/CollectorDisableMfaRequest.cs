using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorDisableMfaRequest
{
    public long UserId { get; set; }

    public CollectorDisableMfaDto FromRequest(long userId) => new() { UserId = userId };
}
