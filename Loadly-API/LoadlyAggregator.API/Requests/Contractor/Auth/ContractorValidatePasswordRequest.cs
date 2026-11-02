using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorValidatePasswordRequest
{
    public string Password { get; set; } = string.Empty;

    public ContractorValidatePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Password = Password
    };
}

public class ContractorValidatePasswordRequestValidator : AbstractValidator<ContractorValidatePasswordRequest>
{
    public ContractorValidatePasswordRequestValidator()
    {
        RuleFor(x => x.Password).IsValidCurrentPassword();
    }
}


