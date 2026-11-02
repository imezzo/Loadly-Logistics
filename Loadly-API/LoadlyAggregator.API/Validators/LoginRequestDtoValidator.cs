using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Shared.DTOs.Auth;

namespace LoadlyAggregator.API.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
        RuleFor(x => x.Password).IsValidPassword();
    }
}

