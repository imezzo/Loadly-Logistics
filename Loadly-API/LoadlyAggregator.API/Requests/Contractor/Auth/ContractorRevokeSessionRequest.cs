using FluentValidation;
using LoadlyAggregator.API.Requests.Contractor.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorRevokeSessionRequest
{
    public long UserId { get; set; }
    public long SessionId { get; set; }

    public ContractorRevokeSessionDto FromRequest() => new()
    {
        UserId = UserId,
        SessionId = SessionId
    };
}

public class ContractorRevokeSessionRequestValidator : AbstractValidator<ContractorRevokeSessionRequest>
{
    public ContractorRevokeSessionRequestValidator()
    {
        RuleFor(x => x.SessionId).IsRequiredId("Session ID");
    }
}
