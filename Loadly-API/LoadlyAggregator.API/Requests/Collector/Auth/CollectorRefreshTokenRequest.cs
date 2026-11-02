using FluentValidation;
using LoadlyAggregator.API.Requests.Collector.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorRefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;

    public RefreshTokenRequestDto FromRequest() => new() { RefreshToken = RefreshToken };
}

public class CollectorRefreshTokenRequestValidator : AbstractValidator<CollectorRefreshTokenRequest>
{
    public CollectorRefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).IsRequiredToken();
    }
}
