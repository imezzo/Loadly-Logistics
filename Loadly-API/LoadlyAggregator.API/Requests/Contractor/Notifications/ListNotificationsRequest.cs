using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Contractor.Notifications;

public class ListNotificationsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public NotificationEventType? EventType { get; set; }
    public NotificationEventType? Exclude { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public ListNotificationsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        EventType = EventType,
        Exclude = Exclude,
        IsRead = IsRead,
        CreatedFrom = CreatedFrom,
        CreatedTo = CreatedTo
    };
}

public class ListNotificationsRequestValidator : AbstractValidator<ListNotificationsRequest>
{
    public ListNotificationsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.EventType).IsInEnum();
        RuleFor(x => x.Exclude).IsInEnum();
    }
}
