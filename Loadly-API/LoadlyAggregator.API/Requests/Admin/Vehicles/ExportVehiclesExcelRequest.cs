using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class ExportVehiclesExcelRequest
{
    public const string Permission = ListVehiclesRequest.Permission;

    public string? Search { get; set; }
    public long? ContractId { get; set; }
    public bool? IsAssignedToContract { get; set; }
    public long? ColorId { get; set; }
    public long? CapacityUnitId { get; set; }
    public long? VehicleTypeId { get; set; }
    public List<long>? VehicleTypeIds { get; set; }
    public bool? PickVehiclesOrTheirHeads { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public List<VehicleTypeCategory>? VehicleTypeCategories { get; set; }
    public int? Status { get; set; }
    public bool? IsAvailableTail { get; set; }
    public long? DriverId { get; set; }
    public List<long>? ContractorIds { get; set; }
    public List<long>? SectorIds { get; set; }
    public List<long>? CompanyIds { get; set; }
    public List<long>? FacilityIds { get; set; }
    public List<VehicleFacilityType>? FacilityTypes { get; set; }
    public bool? HasContractor { get; set; }
    public bool? HasDriver { get; set; }
    public DateTime? TripsStartDateFrom { get; set; }
    public DateTime? TripsEndDateTo { get; set; }
    public bool? OrderByCompletedTripsDescending { get; set; }
    public List<string>? Columns { get; set; }

    public ExportVehiclesExcelDto FromRequest() => new()
    {
        Search = Search,
        ContractId = ContractId,
        IsAssignedToContract = IsAssignedToContract,
        ColorId = ColorId,
        CapacityUnitId = CapacityUnitId,
        VehicleTypeId = VehicleTypeId,
        VehicleTypeIds = VehicleTypeIds,
        PickVehiclesOrTheirHeads = PickVehiclesOrTheirHeads ?? false,
        VehicleUnitType = VehicleUnitType,
        VehicleTypeCategories = VehicleTypeCategories,
        Status = Status,
        IsAvailableTail = IsAvailableTail,
        DriverId = DriverId,
        ContractorIds = ContractorIds,
        SectorIds = SectorIds,
        CompanyIds = CompanyIds,
        FacilityIds = FacilityIds,
        FacilityTypes = FacilityTypes,
        HasContractor = HasContractor,
        HasDriver = HasDriver,
        TripsStartDateFrom = TripsStartDateFrom,
        TripsEndDateTo = TripsEndDateTo,
        OrderByCompletedTripsDescending = OrderByCompletedTripsDescending,
        Columns = Columns
    };
}

public class ExportVehiclesExcelRequestValidator : AbstractValidator<ExportVehiclesExcelRequest>
{
    public ExportVehiclesExcelRequestValidator()
    {
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
        RuleFor(x => x.ContractId).IsOptionalId("Contract ID");
        RuleFor(x => x.VehicleTypeId).IsOptionalId("Vehicle type ID");
        RuleForEach(x => x.VehicleTypeIds)
            .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Vehicle type ID");
        RuleFor(x => x.ColorId).IsOptionalId("Color ID");
        RuleFor(x => x.CapacityUnitId)
            .IsOptionalId("Capacity unit ID")
            .When(x => x.CapacityUnitId.HasValue);
        RuleFor(x => x.DriverId)
            .IsOptionalId("Driver ID")
            .When(x => x.DriverId.HasValue);
        RuleForEach(x => x.ContractorIds)
            .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Contractor ID")
            .When(x => x.ContractorIds is { Count: > 0 });
        RuleForEach(x => x.SectorIds)
            .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Sector ID")
            .When(x => x.SectorIds is { Count: > 0 });
        RuleForEach(x => x.CompanyIds)
            .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Company ID")
            .When(x => x.CompanyIds is { Count: > 0 });
        RuleForEach(x => x.FacilityIds)
            .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Facility ID")
            .When(x => x.FacilityIds is { Count: > 0 });
        RuleForEach(x => x.FacilityTypes)
            .IsValidEnum<ExportVehiclesExcelRequest, VehicleFacilityType>("Location type")
            .When(x => x.FacilityTypes is { Count: > 0 });
        RuleFor(x => x.TripsEndDateTo)
            .GreaterThanOrEqualTo(x => x.TripsStartDateFrom)
            .When(x => x.TripsStartDateFrom.HasValue && x.TripsEndDateTo.HasValue)
            .WithMessage("validation.trips_end_on_or_after_start");
        RuleFor(x => x.Columns)
            .AreValidExportColumnKeys(ExcelExportBuilder.Keys(VehicleExportColumns.All));
    }
}
