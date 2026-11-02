using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class ListDriverCompletedTripsByLifecycleActorRequest : BasePagedRequest
{
    public const string Permission = "view_dashboard";

    public long? DriverId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public ListDriverCompletedTripsByLifecycleActorDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        DriverId = DriverId,
        FromDate = FromDate,
        ToDate = ToDate
    };
}

public class ListDriverCompletedTripsByLifecycleActorRequestValidator : AbstractValidator<ListDriverCompletedTripsByLifecycleActorRequest>
{
    public ListDriverCompletedTripsByLifecycleActorRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();

        RuleFor(x => x.DriverId)
            .IsOptionalId("Driver ID")
            .When(x => x.DriverId.HasValue);

        RuleFor(x => x.ToDate)
            .IsGreaterThanOrEqualToDate(x => x.FromDate!.Value, "End date", "Start date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
    }
}
