using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Validates trip charge rollups against the invoice header at issue and update.
/// </summary>
public static class InvoiceBillingSnapshotApplier
{
    public static void ApplyTripRollupsTo(Invoice invoice, IReadOnlyList<Trip> trips)
    {
        InvoiceTripSubTotalIntegrity.EnsureLiveMatchesHeader(invoice, trips, invoice.InvoiceType);

        var rollups = InvoiceTripChargeRollups.Build(trips, invoice.InvoiceType);
        var linesTotal = InvoiceTripChargeRollups.SumAmount(rollups);
        if (linesTotal != invoice.TripsSubTotal)
        {
            throw new LogicalException(
                "invoices.trip_charges_snapshot_mismatch",
                linesTotal,
                invoice.TripsSubTotal);
        }

        invoice.TripChargesCalculatedAt = DateTime.UtcNow;
    }
}
