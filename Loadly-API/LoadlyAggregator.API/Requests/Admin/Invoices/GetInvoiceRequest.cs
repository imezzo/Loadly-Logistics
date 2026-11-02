using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class GetInvoiceRequest
{
    public const string Permission = "view_invoice";

    public long Id { get; set; }

    public GetInvoiceDto FromRequest() => new() { Id = Id };
}
