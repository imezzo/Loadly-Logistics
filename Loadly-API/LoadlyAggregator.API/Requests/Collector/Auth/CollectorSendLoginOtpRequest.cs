using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorSendLoginOtpRequest
{
    public string Login { get; set; } = string.Empty;

    public CollectorSendLoginOtpDto FromRequest() => new()
    {
        Login = Login
    };
}

public class CollectorSendLoginOtpRequestValidator : AbstractValidator<CollectorSendLoginOtpRequest>
{
    public CollectorSendLoginOtpRequestValidator()
    {
        RuleFor(x => x.Login).IsValidCollectorLogin();
    }
}
