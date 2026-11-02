using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Company.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Company.Invoices;

public class ListInvoicesRequest : BasePagedRequest
{
    public const string Permission = "view_invoice";

    public long? ContractId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public DateOnly? StartDateFrom { get; set; }
    public DateOnly? StartDateTo { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsLeadBilling { get; set; }

    public ListInvoicesDto FromRequest(long companyId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        CompanyId = companyId,
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
        RuleFor(x => x.ContractId).IsOptionalId("Contract ID");
    }
}
