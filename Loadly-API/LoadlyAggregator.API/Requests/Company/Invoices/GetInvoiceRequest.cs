using LoadlyAggregator.Domain.Company.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Company.Invoices;

public class GetInvoiceRequest
{
    public const string Permission = "view_invoice";

    public long Id { get; set; }

    public GetInvoiceDto FromRequest() => new() { Id = Id };
}
