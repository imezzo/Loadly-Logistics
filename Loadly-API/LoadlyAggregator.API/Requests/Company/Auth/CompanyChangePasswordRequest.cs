using FluentValidation;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyChangePasswordRequest
{
    public long UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public CompanyChangePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        CurrentPassword = CurrentPassword,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class CompanyChangePasswordRequestValidator : AbstractValidator<CompanyChangePasswordRequest>
{
    public CompanyChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).IsValidCurrentPassword();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
