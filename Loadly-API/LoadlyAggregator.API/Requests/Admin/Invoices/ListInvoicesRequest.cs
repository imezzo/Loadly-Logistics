using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class ListInvoicesRequest : BasePagedRequest
{
    public const string Permission = "view_invoice";

    public long? CompanyId { get; set; }
    public long? ContractId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public DateOnly? StartDateFrom { get; set; }
    public DateOnly? StartDateTo { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsLeadBilling { get; set; }

    public ListInvoicesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        CompanyId = CompanyId,
        ContractId = ContractId,
        Status = Status,
        InvoiceType = InvoiceType,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo,
        IsOverdue = IsOverdue,
        IsLeadBilling = IsLeadBilling
    };
}

public class ListInvoicesRequestValidator : AbstractValidator<ListInvoicesRequest>
{
    public ListInvoicesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
        RuleFor(x => x.CompanyId).IsOptionalId("Company ID");
        RuleFor(x => x.ContractId).IsOptionalId("Contract ID");
    }
}
