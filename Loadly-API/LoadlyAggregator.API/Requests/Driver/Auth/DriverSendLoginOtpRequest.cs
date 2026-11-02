using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverSendLoginOtpRequest
{
    public string Login { get; set; } = string.Empty;

    public DriverSendLoginOtpDto FromRequest() => new()
    {
        Login = Login
    };
}

public class DriverSendLoginOtpRequestValidator : AbstractValidator<DriverSendLoginOtpRequest>
{
    public DriverSendLoginOtpRequestValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
    }
}
