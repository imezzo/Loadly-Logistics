using FluentValidation;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Requests.Driver.Auth;

public class DriverForgetPasswordRequest
{
    public string Identifier { get; set; } = string.Empty;
    public Shared.Enums.CommunicationChannel Channel { get; set; } = Shared.Enums.CommunicationChannel.Email;

    public DriverForgetPasswordDto FromRequest() => new()
    {
        Identifier = Identifier,
        Channel = Channel
    };
}

public class DriverForgetPasswordRequestValidator : AbstractValidator<DriverForgetPasswordRequest>
{
    public DriverForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Identifier).IsRequiredIdentifier();
        RuleFor(x => x.Channel)
            .IsValidEnum<DriverForgetPasswordRequest, Shared.Enums.CommunicationChannel>("Channel");

        RuleFor(x => x)
            .Must(request => IdentifierValidator.IsValidForChannel(request.Identifier, (Shared.Enums.CommunicationChannel)request.Channel))
            .WithMessage(request => IdentifierValidator.GetErrorMessage((Shared.Enums.CommunicationChannel)request.Channel));
    }
}
