using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanySendLoginOtpRequest
{
    public string Login { get; set; } = string.Empty;

    public CompanySendLoginOtpDto FromRequest() => new()
    {
        Login = Login
    };
}

public class CompanySendLoginOtpRequestValidator : AbstractValidator<CompanySendLoginOtpRequest>
{
    public CompanySendLoginOtpRequestValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
    }
}
