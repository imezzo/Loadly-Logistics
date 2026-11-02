using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverDisableMfaRequest
{
    public long UserId { get; set; }

    public DriverDisableMfaDto FromRequest(long userId) => new() { UserId = userId };
}
