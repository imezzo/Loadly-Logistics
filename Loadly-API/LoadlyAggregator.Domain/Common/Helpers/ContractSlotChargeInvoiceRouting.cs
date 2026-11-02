using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotChargeInvoiceRouting
{
    public static InvoiceType GetEffectiveInvoiceType(
        ContractSlotChargeKind chargeKind,
        InvoiceType extraKmOverageInvoiceType) =>
        chargeKind == ContractSlotChargeKind.ExtraKmOverage
            ? extraKmOverageInvoiceType
            : InvoiceType.Trips;

    public static InvoiceType ResolveExtraKmOverageInvoiceType(ContractBillingSnapshot snapshot) =>
        snapshot.ExtraKmOverageInvoiceType ?? InvoiceType.Trips;

    public static void EnsureExtraKmRoutingMatches(
        ContractBillingSnapshot snapshot,
        InvoiceType requestedExtraKmOverageInvoiceType)
    {
        if (snapshot.ExtraKmOverageInvoiceType is { } stored
            && stored != requestedExtraKmOverageInvoiceType)
        {
            throw new Shared.Exceptions.ValidationException(
                "contract_billing_snapshot.extra_km_routing_mismatch");
        }
    }

    public static InvoiceType LockExtraKmRoutingIfNeeded(
        ContractBillingSnapshot snapshot,
        InvoiceType requestedExtraKmOverageInvoiceType)
    {
        if (snapshot.ExtraKmOverageInvoiceType is { } stored)
            return stored;

        snapshot.ExtraKmOverageInvoiceType = requestedExtraKmOverageInvoiceType;
        return requestedExtraKmOverageInvoiceType;
    }
}
