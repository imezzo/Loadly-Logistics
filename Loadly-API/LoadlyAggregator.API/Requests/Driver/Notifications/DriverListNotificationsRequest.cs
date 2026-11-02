using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Driver.DTOs.Notifications;

namespace LoadlyAggregator.API.Requests.Driver.Notifications;

public class DriverListNotificationsRequest : BasePagedRequest
{
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }

    public DriverListNotificationsDto FromRequest(long driverId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        DriverId = driverId,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo
    };
}

public class DriverListNotificationsRequestValidator : AbstractValidator<DriverListNotificationsRequest>
{
    public DriverListNotificationsRequestValidator()
    {
        RuleFor(x => x.Page)
            .IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");

        RuleFor(x => x.StartDateTo)
            .IsGreaterThanOrEqualToDate(x => x.StartDateFrom!.Value, "End date", "Start date")
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue);
    }
}
