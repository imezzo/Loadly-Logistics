using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminForgetPasswordRequest
{
    public string Identifier { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Sms;

    public AdminForgetPasswordDto FromRequest() => new()
    {
        Identifier = Identifier,
        Channel = Channel
    };
}

public class AdminForgetPasswordRequestValidator : AbstractValidator<AdminForgetPasswordRequest>
{
    public AdminForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Identifier).IsRequiredIdentifier();
        RuleFor(x => x.Channel).MustBeAdminDeliveryChannel();
        RuleFor(x => x)
            .Must(request => IdentifierValidator.IsValidForChannel(request.Identifier, request.Channel))
            .WithMessage(request => IdentifierValidator.GetErrorMessage(request.Channel));
    }
}
