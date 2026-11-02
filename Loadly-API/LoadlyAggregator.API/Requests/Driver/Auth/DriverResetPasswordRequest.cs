using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public DriverResetPasswordDto FromRequest() => new()
    {
        Token = Token,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class DriverResetPasswordRequestValidator : AbstractValidator<DriverResetPasswordRequest>
{
    public DriverResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).IsValidResetToken();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
