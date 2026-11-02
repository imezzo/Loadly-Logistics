using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorSendLoginOtpRequest
{
    public string Login { get; set; } = string.Empty;

    public ContractorSendLoginOtpDto FromRequest() => new()
    {
        Login = Login
    };
}

public class ContractorSendLoginOtpRequestValidator : AbstractValidator<ContractorSendLoginOtpRequest>
{
    public ContractorSendLoginOtpRequestValidator()
    {
        RuleFor(x => x.Login).IsValidLogin();
    }
}
