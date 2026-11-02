using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class ExportInvoicesExcelRequest
{
    public const string Permission = ListInvoicesRequest.Permission;

    public string? Search { get; set; }
    public long? CompanyId { get; set; }
    public long? ContractId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public DateOnly? StartDateFrom { get; set; }
    public DateOnly? StartDateTo { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsLeadBilling { get; set; }
    public List<string>? Columns { get; set; }

    public ExportInvoicesExcelDto FromRequest() => new()
    {
        Search = Search,
        CompanyId = CompanyId,
        ContractId = ContractId,
        Status = Status,
        InvoiceType = InvoiceType,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo,
        IsOverdue = IsOverdue,
        IsLeadBilling = IsLeadBilling,
        Columns = Columns
    };
}

public class ExportInvoicesExcelRequestValidator : AbstractValidator<ExportInvoicesExcelRequest>
{
    public ExportInvoicesExcelRequestValidator()
    {
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
        RuleFor(x => x.CompanyId).IsOptionalId("Company ID");
        RuleFor(x => x.ContractId).IsOptionalId("Contract ID");
        RuleFor(x => x.Columns)
            .AreValidExportColumnKeys(ExcelExportBuilder.Keys(InvoiceExportColumns.All));
    }
}
