using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminRevokeSessionRequest
{
    public long UserId { get; set; }
    public long SessionId { get; set; }

    public AdminRevokeSessionDto FromRequest() => new()
    {
        UserId = UserId,
        SessionId = SessionId
    };
}

public class AdminRevokeSessionRequestValidator : AbstractValidator<AdminRevokeSessionRequest>
{
    public AdminRevokeSessionRequestValidator()
    {
        RuleFor(x => x.SessionId).IsRequiredId("Session ID");
    }
}
