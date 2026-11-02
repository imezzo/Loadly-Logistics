using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Contractor.Trips;

public class ContractorListTripsRequest : BasePagedRequest
{
    public long? VehicleTypeId { get; set; }
    public long? DriverId { get; set; }
    public TripStatus? Status { get; set; }
    public List<TripStatus>? Statuses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public bool? OpenInviteOnly { get; set; }
    public bool? HasContractor { get; set; }
    public bool? IsContractorPaid { get; set; }

    public Domain.Contractor.DTOs.Trips.ContractorListTripsDto FromRequest(long contractorId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractorId = contractorId,
        VehicleTypeId = VehicleTypeId,
        DriverId = DriverId,
        Status = Status,
        Statuses = Statuses,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo,
        OpenInviteOnly = OpenInviteOnly,
        HasContractor = HasContractor,
        IsContractorPaid = IsContractorPaid
    };
}

public class ContractorListTripsRequestValidator : AbstractValidator<ContractorListTripsRequest>
{
    public ContractorListTripsRequestValidator()
    {
        RuleFor(x => x.Page).IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");

        RuleFor(x => x.Search).IsOptionalString(255, "Search term");

        RuleFor(x => x.DriverId).GreaterThan(0).WithMessage("validation.driver_id_gt_zero").When(x => x.DriverId.HasValue);

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status).IsInEnum().WithMessage("validation.trip_status_enum_invalid");
        });

        When(x => x.Statuses is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.Statuses)
                .IsInEnum().WithMessage("validation.trip_status_enum_invalid");
        });

        RuleFor(x => x.StartDateTo)
            .IsGreaterThanOrEqualToDate(x => x.StartDateFrom!.Value, "End date", "Start date")
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue);
    }
}
