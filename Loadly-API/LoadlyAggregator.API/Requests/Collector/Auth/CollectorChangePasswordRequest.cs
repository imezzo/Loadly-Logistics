using FluentValidation;
using LoadlyAggregator.API.Requests.Collector.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorChangePasswordRequest
{
    public long UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public CollectorChangePasswordDto FromRequest(long userId) => new()
    {
        UserId = userId,
        CurrentPassword = CurrentPassword,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class CollectorChangePasswordRequestValidator : AbstractValidator<CollectorChangePasswordRequest>
{
    public CollectorChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).IsValidCurrentPassword();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
