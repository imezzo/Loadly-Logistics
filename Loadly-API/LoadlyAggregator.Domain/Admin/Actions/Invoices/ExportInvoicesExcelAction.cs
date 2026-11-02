using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class ExportInvoicesExcelAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILanguageService _languageService;
    private readonly IExcelParseService _excelParseService;

    public ExportInvoicesExcelAction(
        IInvoiceRepository invoiceRepository,
        ILanguageService languageService,
        IExcelParseService excelParseService)
    {
        _invoiceRepository = invoiceRepository;
        _languageService = languageService;
        _excelParseService = excelParseService;
    }

    public async Task<InvoicesExcelFileDto> ExecuteAsync(ExportInvoicesExcelDto dto)
    {
        (IReadOnlyList<Invoice> invoices, _) = await _invoiceRepository.ListAsync(
            search: dto.Search,
            companyId: dto.CompanyId,
            contractId: dto.ContractId,
            status: dto.Status,
            invoiceType: dto.InvoiceType,
            startDateFrom: dto.StartDateFrom,
            startDateTo: dto.StartDateTo,
            isOverdue: dto.IsOverdue,
            isLeadBilling: dto.IsLeadBilling,
            page: 1,
            pageSize: int.MaxValue);

        IReadOnlyList<ExportColumn<Invoice>> columns = ExcelExportBuilder.ResolveColumns(InvoiceExportColumns.All, dto.Columns);

        return new InvoicesExcelFileDto
        {
            Content = ExcelExportBuilder.Build(
                "Invoices",
                invoices,
                columns,
                _languageService,
                _excelParseService),
            FileName = $"invoices-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx"
        };
    }
}
