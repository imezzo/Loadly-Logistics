using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Trips;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Collector.Trips;

public class CollectorListTripsRequest : BasePagedRequest
{
    public const string Permission = "view_trip";

    public long? ContractId { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public long? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? DriverName { get; set; }
    public string? Plate { get; set; }
    public long? VehicleId { get; set; }
    public long? DriverId { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public long? VehicleTypeId { get; set; }
    public TripStatus? Status { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }

    public CollectorListTripsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractId = ContractId,
        ContractReferenceNumber = ContractReferenceNumber,
        CompanyId = CompanyId,
        CompanyName = CompanyName,
        DriverName = DriverName,
        Plate = Plate,
        VehicleId = VehicleId,
        VehicleTypeId = VehicleTypeId,
        DriverId = DriverId,
        ContractorIds = ContractorIds,
        HasContractor = HasContractor,
        Status = Status,
        TripDocumentStatuses = TripDocumentStatuses,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo
    };
}

public class CollectorListTripsRequestValidator : AbstractValidator<CollectorListTripsRequest>
{
    public CollectorListTripsRequestValidator()
    {
        RuleFor(x => x.Page)
            .IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");

        RuleFor(x => x.ContractId)
            .IsOptionalId("Contract ID")
            .When(x => x.ContractId.HasValue);

        RuleFor(x => x.ContractReferenceNumber)
            .NotEmpty().WithMessage("validation.contract_reference_not_empty")
            .MaximumLength(100).WithMessage("validation.contract_reference_max_length|100")
            .When(x => !string.IsNullOrWhiteSpace(x.ContractReferenceNumber));

        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("validation.company_id_gt_zero")
            .When(x => x.CompanyId.HasValue);

        RuleFor(x => x.CompanyName)
            .IsOptionalString(255, "Company name");

        RuleFor(x => x.DriverName)
            .IsOptionalString(255, "Driver name");

        RuleFor(x => x.Plate)
            .IsOptionalString(50, "Plate");

        RuleFor(x => x.VehicleId)
            .IsOptionalId("Vehicle ID")
            .When(x => x.VehicleId.HasValue);

        RuleFor(x => x.DriverId)
            .GreaterThan(0).WithMessage("validation.driver_id_gt_zero")
            .When(x => x.DriverId.HasValue);

        RuleForEach(x => x.ContractorIds)
            .GreaterThan(0).WithMessage("validation.entity_must_be_greater_than_zero|Contractor ID")
            .When(x => x.ContractorIds is { Count: > 0 });

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

        RuleFor(x => x.StartDateTo)
            .IsGreaterThanOrEqualToDate(x => x.StartDateFrom!.Value, "End date", "Start date")
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue);
    }
}
