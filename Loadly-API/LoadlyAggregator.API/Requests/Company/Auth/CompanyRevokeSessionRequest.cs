using FluentValidation;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyRevokeSessionRequest
{
    public long UserId { get; set; }
    public long SessionId { get; set; }

    public CompanyRevokeSessionDto FromRequest() => new()
    {
        UserId = UserId,
        SessionId = SessionId
    };
}

public class CompanyRevokeSessionRequestValidator : AbstractValidator<CompanyRevokeSessionRequest>
{
    public CompanyRevokeSessionRequestValidator()
    {
        RuleFor(x => x.SessionId).IsRequiredId("Session ID");
    }
}
