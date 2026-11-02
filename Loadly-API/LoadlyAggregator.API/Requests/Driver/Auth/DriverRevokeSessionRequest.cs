using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverRevokeSessionRequest
{
    public long UserId { get; set; }
    public long SessionId { get; set; }

    public DriverRevokeSessionDto FromRequest() => new()
    {
        UserId = UserId,
        SessionId = SessionId
    };
}

public class DriverRevokeSessionRequestValidator : AbstractValidator<DriverRevokeSessionRequest>
{
    public DriverRevokeSessionRequestValidator()
    {
        RuleFor(x => x.SessionId).IsRequiredId("Session ID");
    }
}
