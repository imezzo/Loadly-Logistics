using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminEnableMfaRequest
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Sms;

    public AdminEnableMfaDto FromRequest(long userId) => new()
    {
        UserId = userId,
        Channel = Channel
    };
}

public class AdminEnableMfaRequestValidator : AbstractValidator<AdminEnableMfaRequest>
{
    public AdminEnableMfaRequestValidator()
    {
        RuleFor(x => x.Channel).MustBeAdminDeliveryChannel();
    }
}
