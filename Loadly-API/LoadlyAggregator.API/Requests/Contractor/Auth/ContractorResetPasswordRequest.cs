using FluentValidation;
using LoadlyAggregator.API.Requests.Contractor.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public ContractorResetPasswordDto FromRequest() => new()
    {
        Token = Token,
        NewPassword = NewPassword,
        ConfirmPassword = ConfirmPassword
    };
}

public class ContractorResetPasswordRequestValidator : AbstractValidator<ContractorResetPasswordRequest>
{
    public ContractorResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).IsValidResetToken();
        RuleFor(x => x.NewPassword).IsValidNewPassword();
        RuleFor(x => x.ConfirmPassword)
            .IsValidConfirmPassword()
            .MustMatchPassword(x => x.NewPassword, "Confirmation password", "New password");
    }
}
