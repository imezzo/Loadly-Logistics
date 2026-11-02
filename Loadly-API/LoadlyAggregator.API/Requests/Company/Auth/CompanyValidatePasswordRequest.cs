using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyValidatePasswordRequest
{
    public string Password { get; set; } = string.Empty;

    public CompanyValidatePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Password = Password
    };
}

public class CompanyValidatePasswordRequestValidator : AbstractValidator<CompanyValidatePasswordRequest>
{
    public CompanyValidatePasswordRequestValidator()
    {
        RuleFor(x => x.Password).IsValidCurrentPassword();
    }
}


