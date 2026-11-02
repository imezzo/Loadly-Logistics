using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverChangePasswordRequest
{
    public long UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public DriverChangePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        CurrentPassword = CurrentPassword,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class DriverChangePasswordRequestValidator : AbstractValidator<DriverChangePasswordRequest>
{
    public DriverChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).IsValidCurrentPassword();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
