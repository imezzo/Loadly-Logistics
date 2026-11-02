using LoadlyAggregator.Domain.Company.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Company.Invoices;

public class DownloadTaxedInvoiceAttachmentRequest
{
    public const string Permission = "view_invoice";

    public long Id { get; set; }

    public DownloadTaxedInvoiceAttachmentDto FromRequest() => new() { Id = Id };
}
