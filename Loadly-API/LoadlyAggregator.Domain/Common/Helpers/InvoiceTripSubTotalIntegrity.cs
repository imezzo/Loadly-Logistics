using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class InvoiceTripSubTotalIntegrity
{
    public static decimal ComputeLiveTripsSubTotal(IReadOnlyList<Trip> trips, InvoiceType invoiceType) =>
        trips.Sum(trip => TripChargeInvoiceRouting.SumBillableForInvoiceType(trip, invoiceType));

    /// <summary>
    /// Before persisting a trip snapshot, header <see cref="Invoice.TripsSubTotal"/> must match live trip charges.
    /// </summary>
    public static void EnsureLiveMatchesHeader(Invoice invoice, IReadOnlyList<Trip> trips, InvoiceType invoiceType)
    {
        var live = RoundMoney(ComputeLiveTripsSubTotal(trips, invoiceType));
        var header = RoundMoney(invoice.TripsSubTotal);
        if (live != header)
        {
            throw new LogicalException(
                "invoices.trip_sub_total_live_mismatch",
                header,
                live);
        }
    }

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
