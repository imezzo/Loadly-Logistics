using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class DownloadTaxedInvoiceAttachmentAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMediaService _mediaService;

    public DownloadTaxedInvoiceAttachmentAction(
        IInvoiceRepository invoiceRepository,
        IMediaService mediaService)
    {
        _invoiceRepository = invoiceRepository;
        _mediaService = mediaService;
    }

    public async Task<string> ExecuteAsync(DownloadTaxedInvoiceAttachmentDto dto)
    {
        var invoice = await _invoiceRepository.GetByIdOrThrowAsync(dto.Id);
        if (string.IsNullOrWhiteSpace(invoice.TaxedInvoiceAttachment))
        {
            throw new NotFoundException("invoices.taxed_invoice_attachment_not_found");
        }

        return _mediaService.Read(invoice.TaxedInvoiceAttachment);
    }
}
