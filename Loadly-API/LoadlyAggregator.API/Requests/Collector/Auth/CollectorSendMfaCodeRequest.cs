using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorSendMfaCodeRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Sms;

    public CollectorSendMfaCodeDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}

public class CollectorSendMfaCodeRequestValidator : AbstractValidator<CollectorSendMfaCodeRequest>
{
    public CollectorSendMfaCodeRequestValidator()
    {
        RuleFor(x => x.Channel).IsValidCollectorDeliveryChannel();
    }
}
