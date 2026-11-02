using FluentValidation;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyRefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;

    public RefreshTokenRequestDto FromRequest() => new() { RefreshToken = RefreshToken };
}

public class CompanyRefreshTokenRequestValidator : AbstractValidator<CompanyRefreshTokenRequest>
{
    public CompanyRefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).IsRequiredToken();
    }
}
