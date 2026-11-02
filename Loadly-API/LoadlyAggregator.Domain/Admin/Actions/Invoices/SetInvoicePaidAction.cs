using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class SetInvoicePaidAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public SetInvoicePaidAction(
        IInvoiceRepository invoiceRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(SetInvoicePaidDto dto)
    {
        var invoice = await _invoiceRepository.GetByIdOrThrowAsync(dto.Id);

        if (invoice.Status != InvoiceStatus.UnPaid)
            throw new ValidationException("invoices.cannot_set_non_unpaid_invoice_as_paid");

        invoice.Status = InvoiceStatus.Paid;
        if (invoice.InvoiceDetail is not null)
        {
            invoice.InvoiceDetail.PaidBy = _currentUserService.GetCurrentUserId();
            invoice.InvoiceDetail.PaidAt = DateTime.UtcNow;
            invoice.InvoiceDetail.UpdatedBy = _currentUserService.GetCurrentUserId();
        }

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync();
    }
}
