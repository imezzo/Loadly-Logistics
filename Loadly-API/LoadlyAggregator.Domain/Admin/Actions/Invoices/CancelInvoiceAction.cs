using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class CancelInvoiceAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelInvoiceAction(
        IInvoiceRepository invoiceRepository,
        ITripRepository tripRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(CancelInvoiceDto dto)
    {
        var invoice = await _invoiceRepository.GetByIdOrThrowAsync(dto.Id);

        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new ValidationException("invoices.invoice_already_cancelled");

        if (invoice.Status != InvoiceStatus.UnPaid)
            throw new ValidationException("invoices.cannot_cancel_non_unpaid_invoice");

        if (invoice.Payments?.Any() == true)
            throw new ValidationException("invoices.cannot_cancel_invoice_with_payments");

        var linkedTrips = invoice.InvoiceType == InvoiceType.Claims
            ? invoice.ClaimInvoiceTrips?.ToList() ?? new List<Trip>()
            : invoice.Trips?.ToList() ?? new List<Trip>();

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var trip in linkedTrips)
            {
                if (invoice.InvoiceType == InvoiceType.Claims)
                    trip.ClaimInvoiceId = null;
                else
                    trip.InvoiceId = null;
            }

            if (linkedTrips.Count > 0)
                await _tripRepository.UpdateRangeAsync(linkedTrips);

            if (invoice.ContractBillingSnapshot is { } snapshot)
            {
                var calendarTripIds = ContractBillingSnapshotReader.CollectDistinctTripIds(snapshot);
                var calendarTrips = calendarTripIds.Count == 0
                    ? []
                    : await _tripRepository.GetByIdsAsync(calendarTripIds);
                ContractBillingSnapshotLifecycle.ReopenAfterInvoiceCancellation(
                    snapshot,
                    invoice,
                    calendarTrips);
            }

            invoice.Status = InvoiceStatus.Cancelled;
            invoice.CancelledAt = DateTime.UtcNow;
            invoice.CancellationReason = dto.CancellationReason;
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
