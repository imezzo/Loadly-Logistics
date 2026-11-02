using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class DownloadTaxedInvoiceAttachmentRequest
{
    public const string Permission = "download_invoice_taxed_attachment";

    public long Id { get; set; }

    public DownloadTaxedInvoiceAttachmentDto FromRequest() => new()
    {
        Id = Id
    };
}
