using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverValidatePasswordRequest
{
    public string Password { get; set; } = string.Empty;

    public DriverValidatePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Password = Password
    };
}

public class DriverValidatePasswordRequestValidator : AbstractValidator<DriverValidatePasswordRequest>
{
    public DriverValidatePasswordRequestValidator()
    {
        RuleFor(x => x.Password).IsValidCurrentPassword();
    }
}


