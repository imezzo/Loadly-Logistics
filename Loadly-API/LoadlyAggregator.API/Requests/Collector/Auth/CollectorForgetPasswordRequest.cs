using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorForgetPasswordRequest
{
    public string Identifier { get; set; } = string.Empty;
    public Shared.Enums.CommunicationChannel Channel { get; set; } = Shared.Enums.CommunicationChannel.Sms;

    public CollectorForgetPasswordDto FromRequest() => new()
    {
        Identifier = Identifier,
        Channel = Channel
    };
}

public class CollectorForgetPasswordRequestValidator : AbstractValidator<CollectorForgetPasswordRequest>
{
    public CollectorForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Identifier).IsRequiredEgyptianPhone();
        RuleFor(x => x.Channel).IsValidCollectorDeliveryChannel();
    }
}
