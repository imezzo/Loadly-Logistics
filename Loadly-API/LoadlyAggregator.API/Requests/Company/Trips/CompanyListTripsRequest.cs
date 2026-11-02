using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyListTripsRequest : BasePagedRequest
{
    public long? ContractId { get; set; }
    public long? VehicleId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? DriverId { get; set; }
    public TripStatus? Status { get; set; }
    public OperationalDriverStatus? CurrentDriverStatus { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }

    public Domain.Company.DTOs.Trips.CompanyListTripsDto FromRequest(long companyId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        CompanyId = companyId,
        ContractId = ContractId,
        VehicleId = VehicleId,
        VehicleTypeId = VehicleTypeId,
        DriverId = DriverId,
        Status = Status,
        CurrentDriverStatus = CurrentDriverStatus,
        TripDocumentStatuses = TripDocumentStatuses,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo
    };
}

public class CompanyListTripsRequestValidator : AbstractValidator<CompanyListTripsRequest>
{
    public CompanyListTripsRequestValidator()
    {
        RuleFor(x => x.Page)
            .IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");

        RuleFor(x => x.ContractId)
            .IsOptionalId("Contract ID")
            .When(x => x.ContractId.HasValue);

        RuleFor(x => x.VehicleId)
            .IsOptionalId("Vehicle ID")
            .When(x => x.VehicleId.HasValue);

        RuleFor(x => x.DriverId)
            .GreaterThan(0).WithMessage("validation.driver_id_gt_zero")
            .When(x => x.DriverId.HasValue);

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

        When(x => x.CurrentDriverStatus.HasValue, () =>
        {
            RuleFor(x => x.Status)
                .Equal(TripStatus.InProgress)
                .WithMessage("validation.current_driver_status_requires_in_progress_trip_status");

            RuleFor(x => x.CurrentDriverStatus)
                .IsInEnum()
                .WithMessage("validation.operational_driver_status_enum_invalid");
        });
    }
}
