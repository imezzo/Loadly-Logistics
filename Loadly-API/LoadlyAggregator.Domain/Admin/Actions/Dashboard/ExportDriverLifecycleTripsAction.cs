using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class ExportDriverLifecycleTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ILanguageService _languageService;
    private readonly IExcelParseService _excelParseService;

    public ExportDriverLifecycleTripsAction(
        ITripRepository tripRepository,
        ILanguageService languageService,
        IExcelParseService excelParseService)
    {
        _tripRepository = tripRepository;
        _languageService = languageService;
        _excelParseService = excelParseService;
    }

    public async Task<DriverLifecycleTripsFileDto> ExecuteAsync(ExportDriverLifecycleTripsDto dto)
    {
        var (rows, _) = await _tripRepository.GetDriverCompletedTripsByLifecycleActorAsync(
            dto.DriverId,
            dto.FromDate,
            dto.ToDate,
            page: 1,
            pageSize: int.MaxValue);

        List<DriverCompletedTripsByLifecycleActorDto> exportRows = rows
            .Select(r => new DriverCompletedTripsByLifecycleActorDto
            {
                Id = r.Id,
                NameEn = r.NameEn,
                NameAr = r.NameAr,
                Phone = r.Phone,
                Driver = r.Driver,
                Dispatcher = r.Dispatcher,
                Assisted = r.Assisted,
                Total = r.Total
            })
            .ToList();

        IReadOnlyList<ExportColumn<DriverCompletedTripsByLifecycleActorDto>> columns =
            ExcelExportBuilder.ResolveColumns(DriverLifecycleTripsExportColumns.All, dto.Columns);

        return new DriverLifecycleTripsFileDto
        {
            Content = ExcelExportBuilder.Build(
                "Driver Lifecycle Trips",
                exportRows,
                columns,
                _languageService,
                _excelParseService),
            FileName = $"driver-lifecycle-trips-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx"
        };
    }
}
