using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorEnableMfaRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Sms;

    public CollectorEnableMfaDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}

public class CollectorEnableMfaRequestValidator : AbstractValidator<CollectorEnableMfaRequest>
{
    public CollectorEnableMfaRequestValidator()
    {
        RuleFor(x => x.Channel).IsValidCollectorDeliveryChannel();
    }
}
