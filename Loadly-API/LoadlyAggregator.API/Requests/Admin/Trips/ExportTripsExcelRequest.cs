using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class ExportTripsExcelRequest
{
    public const string Permission = ListTripsRequest.Permission;

    public string? Search { get; set; }
    public long? ContractId { get; set; }
    public ContractType? ContractTypeId { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public List<long>? DestinationIds { get; set; }
    public long? CompanyId { get; set; }
    public long? VehicleId { get; set; }
    public long? DriverId { get; set; }
    public List<long>? LeadIds { get; set; }
    public bool? IsExternalTrip { get; set; }
    public bool? IsBackfill { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public long? VehicleTypeId { get; set; }
    public List<long>? SectorIds { get; set; }
    public TripStatus? Status { get; set; }
    public OperationalDriverStatus? CurrentDriverStatus { get; set; }
    public TripLifecycleActor? LifecycleActor { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public OverNightCostBearer? OverNightCostBearer { get; set; }
    public TripDispatchingMode? DispatchingMode { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateOnly? FinancialDateFrom { get; set; }
    public DateOnly? FinancialDateTo { get; set; }
    public List<string>? Columns { get; set; }

    public ExportTripsExcelDto FromRequest() => new()
    {
        Search = Search,
        ContractId = ContractId,
        ContractTypeId = ContractTypeId,
        ContractReferenceNumber = ContractReferenceNumber,
        DestinationIds = DestinationIds,
        CompanyId = CompanyId,
        VehicleId = VehicleId,
        VehicleTypeId = VehicleTypeId,
        SectorIds = SectorIds,
        DriverId = DriverId,
        LeadIds = LeadIds,
        IsExternalTrip = IsExternalTrip,
        IsBackfill = IsBackfill,
        ContractorIds = ContractorIds,
        HasContractor = HasContractor,
        Status = Status,
        CurrentDriverStatus = CurrentDriverStatus,
        LifecycleActor = LifecycleActor,
        TripDocumentStatuses = TripDocumentStatuses,
        OverNightCostBearer = OverNightCostBearer,
        DispatchingMode = DispatchingMode,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo,
        FinancialDateFrom = FinancialDateFrom,
        FinancialDateTo = FinancialDateTo,
        Columns = Columns
    };
}

public class ExportTripsExcelRequestValidator : AbstractValidator<ExportTripsExcelRequest>
{
    public ExportTripsExcelRequestValidator()
    {
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
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
        RuleFor(x => x.StartDateTo)
            .IsGreaterThanOrEqualToDate(x => x.StartDateFrom!.Value, "End date", "Start date")
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue);
        RuleFor(x => x.FinancialDateTo)
            .GreaterThanOrEqualTo(x => x.FinancialDateFrom)
            .WithMessage("validation.field_must_be_after_or_equal|Financial date to|Financial date from")
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
        RuleFor(x => x.Columns)
            .AreValidExportColumnKeys(ExcelExportBuilder.Keys(TripExportColumns.All));
    }
}
