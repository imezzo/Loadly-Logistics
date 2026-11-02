using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverListTripsRequest : BasePagedRequest
{
    public TripStatus? Status { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }

    public Domain.Driver.DTOs.Trips.DriverListTripsDto FromRequest(long driverId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        DriverId = driverId,
        Status = Status,
        TripDocumentStatuses = TripDocumentStatuses,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo
    };
}

public class DriverListTripsRequestValidator : AbstractValidator<DriverListTripsRequest>
{
    public DriverListTripsRequestValidator()
    {
        RuleFor(x => x.Page)
            .IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");

        // Status validation (optional)
        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("validation.trip_status_enum_invalid");
        });

        When(x => x.TripDocumentStatuses is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.TripDocumentStatuses)
                .IsInEnum().WithMessage("validation.trip_document_status_enum_invalid");
        });

        // Date range validation - if both dates provided, end must be >= start
        RuleFor(x => x.StartDateTo)
            .IsGreaterThanOrEqualToDate(x => x.StartDateFrom!.Value, "End date", "Start date")
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue);
    }
}
