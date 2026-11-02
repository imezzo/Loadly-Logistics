using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminRefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;

    public RefreshTokenRequestDto FromRequest() => new() { RefreshToken = RefreshToken };
}

public class AdminRefreshTokenRequestValidator : AbstractValidator<AdminRefreshTokenRequest>
{
    public AdminRefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).IsRequiredToken();
    }
}
