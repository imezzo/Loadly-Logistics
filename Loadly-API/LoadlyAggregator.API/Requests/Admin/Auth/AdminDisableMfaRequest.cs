using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminDisableMfaRequest
{
    public long UserId { get; set; }

    public AdminDisableMfaDto FromRequest(long userId) => new() { UserId = userId };
}
