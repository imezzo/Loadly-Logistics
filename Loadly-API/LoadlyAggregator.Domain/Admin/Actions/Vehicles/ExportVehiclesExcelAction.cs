using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class ExportVehiclesExcelAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILanguageService _languageService;
    private readonly IExcelParseService _excelParseService;

    public ExportVehiclesExcelAction(
        IVehicleRepository vehicleRepository,
        ILanguageService languageService,
        IExcelParseService excelParseService)
    {
        _vehicleRepository = vehicleRepository;
        _languageService = languageService;
        _excelParseService = excelParseService;
    }

    public async Task<VehiclesExcelFileDto> ExecuteAsync(ExportVehiclesExcelDto dto)
    {
        (IReadOnlyList<Vehicle> vehicles, _) = await _vehicleRepository.ListWithChildrenAsync(
            dto.Search,
            dto.ContractId,
            dto.IsAssignedToContract,
            dto.ColorId,
            dto.CapacityUnitId,
            dto.VehicleTypeId,
            dto.VehicleTypeIds,
            headLookupForTailTypeIds: null,
            dto.VehicleUnitType,
            dto.VehicleTypeCategories,
            dto.Status,
            dto.IsAvailableTail,
            dto.DriverId,
            dto.ContractorIds,
            dto.HasContractor,
            dto.HasDriver,
            dto.SectorIds,
            dto.CompanyIds,
            dto.FacilityIds,
            dto.FacilityTypes,
            dto.OrderByCompletedTripsDescending,
            page: 1,
            pageSize: int.MaxValue);

        IReadOnlyList<ExportColumn<Vehicle>> columns = ExcelExportBuilder.ResolveColumns(VehicleExportColumns.All, dto.Columns);

        return new VehiclesExcelFileDto
        {
            Content = ExcelExportBuilder.Build(
                "Vehicles",
                vehicles,
                columns,
                _languageService,
                _excelParseService),
            FileName = $"vehicles-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx"
        };
    }
}
