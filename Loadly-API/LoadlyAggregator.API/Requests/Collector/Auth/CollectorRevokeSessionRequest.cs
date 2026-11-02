using FluentValidation;
using LoadlyAggregator.API.Requests.Collector.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorRevokeSessionRequest
{
    public long UserId { get; set; }
    public long SessionId { get; set; }

    public CollectorRevokeSessionDto FromRequest() => new()
    {
        UserId = UserId,
        SessionId = SessionId
    };
}

public class CollectorRevokeSessionRequestValidator : AbstractValidator<CollectorRevokeSessionRequest>
{
    public CollectorRevokeSessionRequestValidator()
    {
        RuleFor(x => x.SessionId).IsRequiredId("Session ID");
    }
}
