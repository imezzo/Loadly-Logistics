using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class LinkInvoicePaymentsAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly InvoicePaymentAllocator _allocator;
    private readonly ICurrentUserService _currentUserService;

    public LinkInvoicePaymentsAction(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        InvoicePaymentAllocator allocator,
        ICurrentUserService currentUserService)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _allocator = allocator;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(LinkInvoicePaymentsDto dto)
    {
        var invoice = await _invoiceRepository.GetByIdOrThrowAsync(dto.Id);
        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new ValidationException("invoices.cannot_update_cancelled_invoice");
        if (invoice.Status == InvoiceStatus.Paid)
            throw new ValidationException("invoices.cannot_update_paid_invoice");

        var linkedInvoicePayments = invoice.Payments.Where(p => p.DeletedAt == null).ToList();
        foreach (var item in dto.Payments)
        {
            if (item.InvoicePaymentId is long invoicePaymentId && linkedInvoicePayments.All(p => p.Id != invoicePaymentId))
                throw new NotFoundException("invoice_payments.not_found_with_id", invoicePaymentId);

            var paymentLinkedToBankTransfer = linkedInvoicePayments.FirstOrDefault(p => p.BankTransferId == item.BankTransferId);
            if (paymentLinkedToBankTransfer is not null && paymentLinkedToBankTransfer.Id != item.InvoicePaymentId)
                throw new ValidationException("invoice_payments.bank_transfer_already_linked");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var addedBy = _currentUserService.GetCurrentUserId();
            var requestedInvoicePaymentIds = dto.Payments
                .Where(p => p.InvoicePaymentId.HasValue)
                .Select(p => p.InvoicePaymentId!.Value)
                .ToHashSet();

            var invoicePaymentsToDelete = linkedInvoicePayments
                .Where(p => !requestedInvoicePaymentIds.Contains(p.Id))
                .ToList();
            await _allocator.RemoveRangeAsync(invoice, invoicePaymentsToDelete);

            foreach (var item in dto.Payments.Where(p => p.InvoicePaymentId.HasValue))
                linkedInvoicePayments.First(p => p.Id == item.InvoicePaymentId).BankTransferId = item.BankTransferId;

            // Allocated totals are read from the database, so deletes and bank transfer changes must be flushed first.
            await _unitOfWork.SaveChangesAsync();

            var requestedAllocations = dto.Payments
                .Select(p => (p.BankTransferId, p.Amount))
                .ToList();
            await _allocator.ApplyBatchAsync(invoice, requestedAllocations, addedBy);

            if (invoice.Status == InvoiceStatus.Paid && invoice.InvoiceDetail is not null)
            {
                invoice.InvoiceDetail.PaidBy = addedBy;
                invoice.InvoiceDetail.PaidAt = DateTime.UtcNow;
            }

            _invoiceRepository.Update(invoice);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
