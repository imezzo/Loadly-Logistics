using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using Address = LoadlyAggregator.Domain.Common.Entities.Address;
using City = LoadlyAggregator.Domain.Common.Entities.City;
using DriverDetail = LoadlyAggregator.Domain.Common.Entities.DriverDetail;
using DriverEntity = LoadlyAggregator.Domain.Common.Entities.Driver;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class ExportDriversExcelAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverDetailRepository _driverDetailRepository;
    private readonly ICityRepository _cityRepository;
    private readonly ILanguageService _languageService;
    private readonly IExcelParseService _excelParseService;

    public ExportDriversExcelAction(
        IDriverRepository driverRepository,
        IDriverDetailRepository driverDetailRepository,
        ICityRepository cityRepository,
        ILanguageService languageService,
        IExcelParseService excelParseService)
    {
        _driverRepository = driverRepository;
        _driverDetailRepository = driverDetailRepository;
        _cityRepository = cityRepository;
        _languageService = languageService;
        _excelParseService = excelParseService;
    }

    public async Task<DriversExcelFileDto> ExecuteAsync(ExportDriversExcelDto dto)
    {
        (IReadOnlyList<DriverEntity> drivers, _) = await _driverRepository.ListAsync(
            search: dto.Search,
            status: dto.Status,
            contractorIds: dto.ContractorIds,
            hasContractor: dto.HasContractor,
            isAvailable: dto.IsAvailable,
            licenseCategory: dto.LicenseCategory,
            workingStatuses: dto.WorkingStatuses,
            page: null,
            pageSize: null,
            orderByCompletedTripsDescending: dto.OrderByCompletedTripsDescending);
        IReadOnlyList<long> driverIds = drivers.Select(driver => driver.Id).ToList();
        IReadOnlyDictionary<long, DriverDetail> detailByDriverId =
            await _driverDetailRepository.GetMapByDriverIdsAsync(driverIds);

        HashSet<long> cityIds = [];
        foreach (DriverEntity driver in drivers)
        {
            Address? address = driver.Addresses.FirstOrDefault();
            if (address == null)
                continue;
            cityIds.Add(address.CityId);
        }

        IReadOnlyDictionary<long, City> cityById = await _cityRepository.GetMapByIdsAsync(cityIds);
        List<DriverExportRow> rows = drivers.Select(driver => MapExportRow(driver, detailByDriverId, cityById)).ToList();
        IReadOnlyList<ExportColumn<DriverExportRow>> columns = ExcelExportBuilder.ResolveColumns(DriverExportColumns.All, dto.Columns);

        return new DriversExcelFileDto
        {
            Content = ExcelExportBuilder.Build(
                "Drivers",
                rows,
                columns,
                _languageService,
                _excelParseService),
            FileName = $"drivers-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx"
        };
    }

    private DriverExportRow MapExportRow(
        DriverEntity driver,
        IReadOnlyDictionary<long, DriverDetail> detailByDriverId,
        IReadOnlyDictionary<long, City> cityById)
    {
        detailByDriverId.TryGetValue(driver.Id, out DriverDetail? detail);
        string cityName = driver.Addresses.FirstOrDefault() is Address address
            && cityById.TryGetValue(address.CityId, out City? city)
            ? _languageService.IsArabic()
                ? city.NameAr ?? string.Empty
                : city.NameEn ?? string.Empty
            : string.Empty;

        return new DriverExportRow(
            driver.UserName,
            _languageService.IsArabic() ? driver.FullNameAr ?? string.Empty : driver.FullName ?? string.Empty,
            driver.NationalId ?? string.Empty,
            driver.Phone,
            driver.Email,
            cityName,
            detail?.LicenseNo ?? string.Empty);
    }
}
