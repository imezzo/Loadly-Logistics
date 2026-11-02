using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public AdminResetPasswordDto FromRequest() => new()
    {
        Token = Token,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class AdminResetPasswordRequestValidator : AbstractValidator<AdminResetPasswordRequest>
{
    public AdminResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).IsValidResetToken();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
