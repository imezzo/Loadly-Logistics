using FluentValidation;
using LoadlyAggregator.API.Requests.Contractor.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class ContractorForgetPasswordRequest
{
    public string Identifier { get; set; } = string.Empty;
    public Shared.Enums.CommunicationChannel Channel { get; set; } = Shared.Enums.CommunicationChannel.Email;

    public ContractorForgetPasswordDto FromRequest() => new()
    {
        Identifier = Identifier,
        Channel = Channel
    };
}

public class ContractorForgetPasswordRequestValidator : AbstractValidator<ContractorForgetPasswordRequest>
{
    public ContractorForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Identifier).IsRequiredIdentifier();
        RuleFor(x => x.Channel)
            .IsValidEnum<ContractorForgetPasswordRequest, Shared.Enums.CommunicationChannel>("Channel");

        RuleFor(x => x)
            .Must(request => IdentifierValidator.IsValidForChannel(request.Identifier, (Shared.Enums.CommunicationChannel)request.Channel))
            .WithMessage(request => IdentifierValidator.GetErrorMessage((Shared.Enums.CommunicationChannel)request.Channel));
    }
}
