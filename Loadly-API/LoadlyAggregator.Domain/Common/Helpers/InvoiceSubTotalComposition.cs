using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class InvoiceSubTotalComposition
{
    public static void EnsureValid(decimal slotChargesSubTotal, decimal tripsSubTotal, decimal subTotal)
    {
        var expected = RoundMoney(slotChargesSubTotal + tripsSubTotal);
        var actual = RoundMoney(subTotal);
        if (expected != actual)
        {
            throw new LogicalException(
                "invoices.sub_total_composition_mismatch",
                actual,
                slotChargesSubTotal,
                tripsSubTotal);
        }
    }

    public static void EnsureValid(Invoice invoice) =>
        EnsureValid(invoice.SlotChargesSubTotal, invoice.TripsSubTotal, invoice.SubTotal);

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
