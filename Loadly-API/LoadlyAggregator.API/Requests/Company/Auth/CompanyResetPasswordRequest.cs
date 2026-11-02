using FluentValidation;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public CompanyResetPasswordDto FromRequest() => new()
    {
        Token = Token,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class CompanyResetPasswordRequestValidator : AbstractValidator<CompanyResetPasswordRequest>
{
    public CompanyResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).IsValidResetToken();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
