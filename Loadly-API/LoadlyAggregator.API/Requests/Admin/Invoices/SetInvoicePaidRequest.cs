using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class SetInvoicePaidRequest
{
    public const string Permission = "edit_invoices";

    public long Id { get; set; }

    public SetInvoicePaidDto FromRequest() => new() { Id = Id };
}
