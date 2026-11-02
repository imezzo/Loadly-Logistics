using FluentValidation;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Common.Notifications;

public class GetUnreadCountRequest
{
    public NotificationEventType? EventType { get; set; }
    public NotificationEventType? Exclude { get; set; }
}

public class GetUnreadCountRequestValidator : AbstractValidator<GetUnreadCountRequest>
{
    public GetUnreadCountRequestValidator()
    {
        RuleFor(x => x.EventType).IsInEnum();
        RuleFor(x => x.Exclude).IsInEnum();
    }
}
