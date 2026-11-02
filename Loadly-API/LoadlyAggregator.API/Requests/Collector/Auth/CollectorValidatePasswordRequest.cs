using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorValidatePasswordRequest
{
    public string Password { get; set; } = string.Empty;

    public CollectorValidatePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Password = Password
    };
}

public class CollectorValidatePasswordRequestValidator : AbstractValidator<CollectorValidatePasswordRequest>
{
    public CollectorValidatePasswordRequestValidator()
    {
        RuleFor(x => x.Password).IsValidCurrentPassword();
    }
}


