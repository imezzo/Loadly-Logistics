using FluentValidation;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyForgetPasswordRequest
{
    public string Identifier { get; set; } = string.Empty;
    public Shared.Enums.CommunicationChannel Channel { get; set; } = Shared.Enums.CommunicationChannel.Email;

    public CompanyForgetPasswordDto FromRequest() => new()
    {
        Identifier = Identifier,
        Channel = Channel
    };
}

public class CompanyForgetPasswordRequestValidator : AbstractValidator<CompanyForgetPasswordRequest>
{
    public CompanyForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Identifier).IsRequiredIdentifier();
        RuleFor(x => x.Channel)
            .IsValidEnum<CompanyForgetPasswordRequest, Shared.Enums.CommunicationChannel>("Channel");

        RuleFor(x => x)
            .Must(request => IdentifierValidator.IsValidForChannel(request.Identifier, (Shared.Enums.CommunicationChannel)request.Channel))
            .WithMessage(request => IdentifierValidator.GetErrorMessage((Shared.Enums.CommunicationChannel)request.Channel));
    }
}
