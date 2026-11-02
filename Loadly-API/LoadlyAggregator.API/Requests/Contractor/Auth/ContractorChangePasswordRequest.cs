using FluentValidation;
using LoadlyAggregator.API.Requests.Contractor.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorChangePasswordRequest
{
    public long UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public ContractorChangePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        CurrentPassword = CurrentPassword,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class ContractorChangePasswordRequestValidator : AbstractValidator<ContractorChangePasswordRequest>
{
    public ContractorChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).IsValidCurrentPassword();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
