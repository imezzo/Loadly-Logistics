using ClosedXML.Excel;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class ExportTripsExcelAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ICompanyDetailRepository _companyDetailRepository;
    private readonly ILanguageService _languageService;
    private readonly IExcelParseService _excelParseService;

    public ExportTripsExcelAction(
        ITripRepository tripRepository,
        ICompanyDetailRepository companyDetailRepository,
        ILanguageService languageService,
        IExcelParseService excelParseService)
    {
        _tripRepository = tripRepository;
        _companyDetailRepository = companyDetailRepository;
        _languageService = languageService;
        _excelParseService = excelParseService;
    }

    public async Task<TripsExcelFileDto> ExecuteAsync(ExportTripsExcelDto dto)
    {
        List<long>? matchingCompanyIds = null;
        if (!string.IsNullOrWhiteSpace(dto.Search))
        {
            string searchTerm = dto.Search.Trim();
            var matchingCompanyDetails = await _companyDetailRepository.FindAsync(cd =>
                cd.NameEn.ToLower().Contains(searchTerm.ToLower()) ||
                (cd.NameAr != null && cd.NameAr.ToLower().Contains(searchTerm.ToLower())));
            matchingCompanyIds = matchingCompanyDetails.Select(cd => cd.CompanyId).ToList();
        }

        int[] visibleStatuses = Enum.GetValues<TripStatus>()
            .Where(status => status != TripStatus.CompanyDraft)
            .Select(status => (int)status)
            .ToArray();
        IReadOnlyList<ExportColumn<TripExportRow>> columns = ExcelExportBuilder.ResolveColumns(TripExportColumns.All, dto.Columns);
        string[] columnKeys = columns.Select(column => column.Key).ToArray();

        using XLWorkbook workbook = new();
        IXLWorksheet sheet = workbook.Worksheets.Add("Trips");
        ExcelExportBuilder.WriteHeader(sheet, columns, _languageService);

        int row = 2;
        await foreach (IReadOnlyList<TripExportRow> batch in _tripRepository.EnumerateForExportAsync(
            BuildCriteria(dto, matchingCompanyIds, visibleStatuses),
            columnKeys))
        {
            row = ExcelExportBuilder.AppendRows(sheet, row, batch, columns, _languageService);
        }

        bool isArabic = _languageService.IsArabic();
        _excelParseService.ApplyExportStyle(
            sheet,
            columns.Where(column => column.Multiline).Select(column => isArabic ? column.LabelAr : column.LabelEn).ToList());

        using MemoryStream stream = new();
        workbook.SaveAs(stream);
        return new TripsExcelFileDto
        {
            Content = stream.ToArray(),
            FileName = $"trips-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx"
        };
    }

    private static TripExportCriteria BuildCriteria(
        ExportTripsExcelDto dto,
        List<long>? matchingCompanyIds,
        int[] visibleStatuses) => new()
    {
        Search = dto.Search,
        ContractId = dto.ContractId,
        ContractTypeId = dto.ContractTypeId,
        ContractReferenceNumber = dto.ContractReferenceNumber,
        DestinationIds = dto.DestinationIds,
        CompanyId = dto.CompanyId,
        MatchingCompanyIds = matchingCompanyIds,
        VehicleId = dto.VehicleId,
        VehicleTypeId = dto.VehicleTypeId,
        SectorIds = dto.SectorIds,
        DriverId = dto.DriverId,
        LeadIds = dto.LeadIds,
        IsExternalTrip = dto.IsExternalTrip,
        IsBackfill = dto.IsBackfill,
        ContractorIds = dto.ContractorIds,
        HasContractor = dto.HasContractor,
        Status = dto.Status.HasValue ? (int)dto.Status.Value : null,
        Statuses = visibleStatuses,
        CurrentDriverStatus = dto.CurrentDriverStatus,
        LifecycleActor = dto.LifecycleActor,
        TripDocumentStatuses = dto.TripDocumentStatuses,
        OverNightCostBearer = dto.OverNightCostBearer.HasValue ? (int)dto.OverNightCostBearer.Value : null,
        DispatchingMode = dto.DispatchingMode.HasValue ? (int)dto.DispatchingMode.Value : null,
        StartDateFrom = dto.StartDateFrom,
        StartDateTo = dto.StartDateTo,
        FinancialDateFrom = dto.FinancialDateFrom,
        FinancialDateTo = dto.FinancialDateTo
    };
}
