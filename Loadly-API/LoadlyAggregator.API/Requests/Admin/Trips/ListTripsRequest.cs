using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Trips;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class ListTripsRequest : BasePagedRequest
{
    public const string Permission = "view_trip";

    public long? ContractId { get; set; }
    public ContractType? ContractTypeId { get; set; }
    public string? ContractReferenceNumber { get; set; }  // Filter by contract reference number
    public List<long>? DestinationIds { get; set; }
    public long? CompanyId { get; set; }  // Filter by company ID
    public long? VehicleId { get; set; }
    public long? DriverId { get; set; }  // Filter by driver ID
    public List<long>? LeadIds { get; set; }
    public bool? IsExternalTrip { get; set; }
    public bool? IsBackfill { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? ContractVehicleTypeId { get; set; }
    public bool? IsCreatedByCompany { get; set; }
    public List<long>? SectorIds { get; set; }
    public TripStatus? Status { get; set; }  // Filter by trip status
    public OperationalDriverStatus? CurrentDriverStatus { get; set; }
    public List<TripStatus>? Statuses { get; set; }  // Filter by multiple trip statuses
    public TripLifecycleActor? LifecycleActor { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public OverNightCostBearer? OverNightCostBearer { get; set; }
    public TripDispatchingMode? DispatchingMode { get; set; }
    public bool? IsPushedToContractors { get; set; }
    public bool? HasContractorInvoice { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateOnly? FinancialDateFrom { get; set; }
    public DateOnly? FinancialDateTo { get; set; }

    public ListTripsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractId = ContractId,
        ContractTypeId = ContractTypeId,
        ContractReferenceNumber = ContractReferenceNumber,
        DestinationIds = DestinationIds,
        CompanyId = CompanyId,
        VehicleId = VehicleId,
        VehicleTypeId = VehicleTypeId,
        ContractVehicleTypeId = ContractVehicleTypeId,
        IsCreatedByCompany = IsCreatedByCompany,
        SectorIds = SectorIds,
        DriverId = DriverId,
        LeadIds = LeadIds,
        IsExternalTrip = IsExternalTrip,
        IsBackfill = IsBackfill,
        ContractorIds = ContractorIds,
        HasContractor = HasContractor,
        Status = Status,
        Statuses = Statuses,
        CurrentDriverStatus = CurrentDriverStatus,
        LifecycleActor = LifecycleActor,
        TripDocumentStatuses = TripDocumentStatuses,
        OverNightCostBearer = OverNightCostBearer,
        DispatchingMode = DispatchingMode,
        IsPushedToContractors = IsPushedToContractors,
        HasContractorInvoice = HasContractorInvoice,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo,
        FinancialDateFrom = FinancialDateFrom,
        FinancialDateTo = FinancialDateTo
    };
}

public class ListTripsRequestValidator : AbstractValidator<ListTripsRequest>
{
    public ListTripsRequestValidator()
    {
        RuleFor(x => x.Page)
            .IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(50000).WithMessage("validation.page_size_must_not_exceed|50000");

        RuleFor(x => x.ContractId)
            .IsOptionalId("Contract ID")
            .When(x => x.ContractId.HasValue);

        RuleFor(x => x.ContractTypeId)
            .IsInEnum()
            .When(x => x.ContractTypeId.HasValue);

        RuleFor(x => x.ContractReferenceNumber)
            .NotEmpty().WithMessage("validation.contract_reference_not_empty")
            .MaximumLength(100).WithMessage("validation.contract_reference_max_length|100")
            .When(x => !string.IsNullOrWhiteSpace(x.ContractReferenceNumber));

        RuleForEach(x => x.DestinationIds)
            .GreaterThan(0).WithMessage("validation.entity_must_be_greater_than_zero|Destination ID")
            .When(x => x.DestinationIds is { Count: > 0 });

        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("validation.company_id_gt_zero")
            .When(x => x.CompanyId.HasValue);

        RuleFor(x => x.VehicleId)
            .IsOptionalId("Vehicle ID")
            .When(x => x.VehicleId.HasValue);

        RuleFor(x => x.ContractVehicleTypeId)
            .IsOptionalId("Contract vehicle type ID")
            .When(x => x.ContractVehicleTypeId.HasValue);

        RuleForEach(x => x.SectorIds)
            .GreaterThan(0).WithMessage("validation.entity_must_be_greater_than_zero|Sector ID")
            .When(x => x.SectorIds is { Count: > 0 });

        RuleFor(x => x.DriverId)
            .GreaterThan(0).WithMessage("validation.driver_id_gt_zero")
            .When(x => x.DriverId.HasValue);

        When(x => x.LeadIds is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.LeadIds)
                .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Lead ID");
        });

        RuleForEach(x => x.ContractorIds)
            .GreaterThan(0).WithMessage("validation.entity_must_be_greater_than_zero|Contractor ID")
            .When(x => x.ContractorIds is { Count: > 0 });

        // Status validation (optional)
        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("validation.trip_status_enum_invalid");
        });

        When(x => x.Statuses is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.Statuses)
                .IsInEnum().WithMessage("validation.trip_status_enum_invalid");
        });

        When(x => x.TripDocumentStatuses is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.TripDocumentStatuses)
                .IsInEnum().WithMessage("validation.trip_document_status_enum_invalid");
        });

        When(x => x.OverNightCostBearer.HasValue, () =>
        {
            RuleFor(x => x.OverNightCostBearer)
                .IsInEnum().WithMessage("validation.trip_status_enum_invalid");
        });

        When(x => x.DispatchingMode.HasValue, () =>
        {
            RuleFor(x => x.DispatchingMode)
                .IsInEnum().WithMessage("validation.trip_dispatching_mode_enum_invalid");
        });

        // Date range validation - if both dates provided, end must be >= start
        RuleFor(x => x.StartDateTo)
            .IsGreaterThanOrEqualToDate(x => x.StartDateFrom!.Value, "End date", "Start date")
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue);

        RuleFor(x => x.FinancialDateTo)
            .GreaterThanOrEqualTo(x => x.FinancialDateFrom)
            .WithMessage("validation.field_must_be_after_or_equal|Financial end date|Financial start date")
            .When(x => x.FinancialDateFrom.HasValue && x.FinancialDateTo.HasValue);

        When(x => x.CurrentDriverStatus.HasValue, () =>
        {
            RuleFor(x => x.Status)
                .Equal(TripStatus.InProgress)
                .WithMessage("validation.current_driver_status_requires_in_progress_trip_status");

            RuleFor(x => x.CurrentDriverStatus)
                .IsInEnum()
                .WithMessage("validation.operational_driver_status_enum_invalid");
        });

        When(x => x.LifecycleActor.HasValue, () =>
        {
            RuleFor(x => x.LifecycleActor)
                .IsInEnum()
                .WithMessage("validation.lifecycle_actor_enum_invalid");
        });
    }
}
