using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverRefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;

    public RefreshTokenRequestDto FromRequest() => new() { RefreshToken = RefreshToken };
}

public class DriverRefreshTokenRequestValidator : AbstractValidator<DriverRefreshTokenRequest>
{
    public DriverRefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).IsRequiredToken();
    }
}
