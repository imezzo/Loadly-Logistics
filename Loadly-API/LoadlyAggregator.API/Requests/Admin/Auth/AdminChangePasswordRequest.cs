using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminChangePasswordRequest
{
    public long UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public AdminChangePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        CurrentPassword = CurrentPassword,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class AdminChangePasswordRequestValidator : AbstractValidator<AdminChangePasswordRequest>
{
    public AdminChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).IsValidCurrentPassword();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
