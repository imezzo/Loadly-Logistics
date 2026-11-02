using FluentValidation;
using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Common.Notifications;

public class UpdateNotificationPreferenceRequest
{
    public NotificationChannel Channel { get; set; }
    public NotificationEventType EventType { get; set; }
    public bool IsEnabled { get; set; }

    public UpdateNotificationPreferenceDto FromRequest() => new()
    {
        Channel = Channel,
        EventType = EventType,
        IsEnabled = IsEnabled
    };
}

public class UpdateNotificationPreferenceRequestValidator : AbstractValidator<UpdateNotificationPreferenceRequest>
{
    public UpdateNotificationPreferenceRequestValidator()
    {
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.EventType).IsInEnum();
    }
}
