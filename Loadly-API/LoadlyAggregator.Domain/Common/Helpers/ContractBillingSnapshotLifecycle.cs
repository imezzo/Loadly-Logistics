using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotLifecycle
{
    public static void StageInvoiceAgainstSnapshot(ContractBillingSnapshot snapshot, Invoice invoice)
    {
        if (snapshot.Status == ContractBillingSnapshotStatus.Finalized)
            throw new LogicalException("contract_billing_snapshot.already_finalized");

        if (invoice.InvoiceType == InvoiceType.Trips)
        {
            if (snapshot.InvoiceId is > 0)
                throw new LogicalException("contract_billing_snapshot.trips_invoice_already_exists");
        }
        else if (snapshot.ClaimInvoiceId is > 0)
        {
            throw new LogicalException("contract_billing_snapshot.claims_invoice_already_exists");
        }

        InvoiceSlotChargeHeaderSync.ApplyFromSnapshot(invoice, snapshot);
    }

    public static void LinkPersistedInvoice(
        ContractBillingSnapshot snapshot,
        Invoice invoice,
        IReadOnlyList<Trip> calendarTrips)
    {
        if (invoice.Id <= 0)
            throw new LogicalException("invoices.invoice_must_be_persisted_before_snapshot_link");

        if (invoice.InvoiceType == InvoiceType.Trips)
        {
            if (snapshot.InvoiceId is > 0 && snapshot.InvoiceId != invoice.Id)
                throw new LogicalException("contract_billing_snapshot.trips_invoice_already_exists");
            snapshot.InvoiceId = invoice.Id;
        }
        else
        {
            if (snapshot.ClaimInvoiceId is > 0 && snapshot.ClaimInvoiceId != invoice.Id)
                throw new LogicalException("contract_billing_snapshot.claims_invoice_already_exists");
            snapshot.ClaimInvoiceId = invoice.Id;
        }

        if (snapshot.FinalizedAt is null)
            snapshot.FinalizedAt = DateTime.UtcNow;

        snapshot.Status = ContractBillingSnapshotInvoicingProgress.ResolveStatusAfterInvoiceChange(
            snapshot,
            calendarTrips);
    }

    public static void ReopenAfterInvoiceCancellation(
        ContractBillingSnapshot snapshot,
        Invoice invoice,
        IReadOnlyList<Trip> calendarTrips)
    {
        if (invoice.ContractBillingSnapshotId != snapshot.Id)
            return;

        if (invoice.InvoiceType == InvoiceType.Trips)
        {
            if (snapshot.InvoiceId is > 0 && snapshot.InvoiceId != invoice.Id)
                return;
            snapshot.InvoiceId = null;
        }
        else
        {
            if (snapshot.ClaimInvoiceId is > 0 && snapshot.ClaimInvoiceId != invoice.Id)
                return;
            snapshot.ClaimInvoiceId = null;
        }

        if (snapshot.InvoiceId is null && snapshot.ClaimInvoiceId is null)
        {
            snapshot.Status = ContractBillingSnapshotStatus.Draft;
            snapshot.FinalizedAt = null;
            snapshot.ExtraKmOverageInvoiceType = null;
            return;
        }

        snapshot.Status = ContractBillingSnapshotInvoicingProgress.ResolveStatusAfterInvoiceChange(
            snapshot,
            calendarTrips);
    }

    public static void EnsureMatchesContract(ContractBillingSnapshot snapshot, long contractId)
    {
        if (snapshot.ContractId != contractId)
            throw new ValidationException("contract_billing_snapshot.contract_mismatch");
    }

    public static void EnsureMatchesInvoiceRequest(
        ContractBillingSnapshot snapshot,
        long contractId,
        DateOnly periodStart,
        DateOnly periodEnd)
    {
        EnsureMatchesContract(snapshot, contractId);
        if (snapshot.PeriodStart != periodStart || snapshot.PeriodEnd != periodEnd)
            throw new ValidationException("contract_billing_snapshot.period_mismatch");
    }

    public static CalculateIncludedContractSlotChargesResultDto ToCalculationDto(ContractBillingSnapshot snapshot) =>
        ContractBillingSnapshotReader.ToCalculationDto(snapshot);
}
