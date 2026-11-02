using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminValidatePasswordRequest
{
    public string Password { get; set; } = string.Empty;

    public AdminValidatePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Password = Password
    };
}

public class AdminValidatePasswordRequestValidator : AbstractValidator<AdminValidatePasswordRequest>
{
    public AdminValidatePasswordRequestValidator()
    {
        RuleFor(x => x.Password).IsValidCurrentPassword();
    }
}


