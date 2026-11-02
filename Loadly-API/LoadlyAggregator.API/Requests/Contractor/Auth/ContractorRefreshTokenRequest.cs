using FluentValidation;
using LoadlyAggregator.API.Requests.Contractor.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorRefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;

    public RefreshTokenRequestDto FromRequest() => new() { RefreshToken = RefreshToken };
}

public class ContractorRefreshTokenRequestValidator : AbstractValidator<ContractorRefreshTokenRequest>
{
    public ContractorRefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).IsRequiredToken();
    }
}
