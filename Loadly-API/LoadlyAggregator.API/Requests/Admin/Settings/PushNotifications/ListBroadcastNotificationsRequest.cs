using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.PushNotifications;

public class ListBroadcastNotificationsRequest : BasePagedRequest
{
    public const string Permission = "view_broadcast_notification";

    public ListBroadcastNotificationsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListBroadcastNotificationsRequestValidator : AbstractValidator<ListBroadcastNotificationsRequest>
{
    public ListBroadcastNotificationsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
