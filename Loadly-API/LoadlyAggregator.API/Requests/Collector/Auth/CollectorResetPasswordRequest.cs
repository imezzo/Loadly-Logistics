using FluentValidation;
using LoadlyAggregator.API.Requests.Collector.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public CollectorResetPasswordDto FromRequest() => new()
    {
        Token = Token,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class CollectorResetPasswordRequestValidator : AbstractValidator<CollectorResetPasswordRequest>
{
    public CollectorResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).IsValidResetToken();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
