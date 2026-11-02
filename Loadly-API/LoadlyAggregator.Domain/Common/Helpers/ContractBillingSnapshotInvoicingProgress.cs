using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotInvoicingProgress
{
    public static bool IsBillingLocked(ContractBillingSnapshotStatus status) =>
        status is ContractBillingSnapshotStatus.Finalized
            or ContractBillingSnapshotStatus.PartiallyInvoiced;

    public static void EnsureDraftForMutation(ContractBillingSnapshotStatus status)
    {
        if (IsBillingLocked(status))
            throw new Shared.Exceptions.ValidationException("contract_billing_snapshot.not_draft");
    }

    public static bool RequiresTripsInvoice(
        ContractBillingSnapshot snapshot,
        InvoiceType? satisfiedByPendingInvoiceType = null)
    {
        if (snapshot.InvoiceId is > 0 || satisfiedByPendingInvoiceType == InvoiceType.Trips)
            return false;

        var rollups = ContractBillingSnapshotSlotChargeRollups.ForInvoiceType(snapshot, InvoiceType.Trips);
        return rollups.SubTotal > 0m;
    }

    public static bool RequiresClaimsInvoice(
        ContractBillingSnapshot snapshot,
        InvoiceType? satisfiedByPendingInvoiceType = null)
    {
        if (snapshot.ClaimInvoiceId is > 0 || satisfiedByPendingInvoiceType == InvoiceType.Claims)
            return false;

        var rollups = ContractBillingSnapshotSlotChargeRollups.ForInvoiceType(snapshot, InvoiceType.Claims);
        return rollups.SubTotal > 0m;
    }

    public static bool RequiresTripsInvoiceForTrips(
        ContractBillingSnapshot snapshot,
        IReadOnlyList<Trip> trips,
        InvoiceType? satisfiedByPendingInvoiceType = null) =>
        satisfiedByPendingInvoiceType != InvoiceType.Trips
        && trips.Any(trip => HasUninvoicedBillableAmount(trip, InvoiceType.Trips));

    public static bool RequiresClaimsInvoiceForTrips(
        ContractBillingSnapshot snapshot,
        IReadOnlyList<Trip> trips,
        InvoiceType? satisfiedByPendingInvoiceType = null) =>
        satisfiedByPendingInvoiceType != InvoiceType.Claims
        && trips.Any(trip => HasUninvoicedBillableAmount(trip, InvoiceType.Claims));

    private static bool HasUninvoicedBillableAmount(Trip trip, InvoiceType invoiceType) =>
        invoiceType switch
        {
            InvoiceType.Trips => trip.InvoiceId is not > 0
                && TripChargeInvoiceRouting.HasBillableAmount(trip, InvoiceType.Trips),
            InvoiceType.Claims => trip.ClaimInvoiceId is not > 0
                && TripChargeInvoiceRouting.HasBillableAmount(trip, InvoiceType.Claims),
            _ => false
        };

    public static bool IsSlotInvoicingComplete(
        ContractBillingSnapshot snapshot,
        InvoiceType? satisfiedByPendingInvoiceType = null) =>
        !RequiresTripsInvoice(snapshot, satisfiedByPendingInvoiceType)
        && !RequiresClaimsInvoice(snapshot, satisfiedByPendingInvoiceType);

    public static bool IsTripInvoicingComplete(
        ContractBillingSnapshot snapshot,
        IReadOnlyList<Trip> trips,
        InvoiceType? satisfiedByPendingInvoiceType = null) =>
        !RequiresTripsInvoiceForTrips(snapshot, trips, satisfiedByPendingInvoiceType)
        && !RequiresClaimsInvoiceForTrips(snapshot, trips, satisfiedByPendingInvoiceType);

    public static bool IsFullyInvoiced(
        ContractBillingSnapshot snapshot,
        IReadOnlyList<Trip> calendarTrips,
        InvoiceType? satisfiedByPendingInvoiceType = null) =>
        IsSlotInvoicingComplete(snapshot, satisfiedByPendingInvoiceType)
        && IsTripInvoicingComplete(snapshot, calendarTrips, satisfiedByPendingInvoiceType);

    public static ContractBillingSnapshotStatus ResolveStatusAfterInvoiceChange(
        ContractBillingSnapshot snapshot,
        IReadOnlyList<Trip> calendarTrips,
        InvoiceType? satisfiedByPendingInvoiceType = null) =>
        IsFullyInvoiced(snapshot, calendarTrips, satisfiedByPendingInvoiceType)
            ? ContractBillingSnapshotStatus.Finalized
            : ContractBillingSnapshotStatus.PartiallyInvoiced;

    public static void EnsureCanCreateInvoice(
        ContractBillingSnapshot snapshot,
        InvoiceType invoiceType)
    {
        if (snapshot.Status == ContractBillingSnapshotStatus.Finalized)
            throw new Shared.Exceptions.ValidationException("contract_billing_snapshot.already_finalized");

        if (invoiceType == InvoiceType.Trips && snapshot.InvoiceId is > 0)
            throw new Shared.Exceptions.ValidationException("contract_billing_snapshot.trips_invoice_already_exists");

        if (invoiceType == InvoiceType.Claims && snapshot.ClaimInvoiceId is > 0)
            throw new Shared.Exceptions.ValidationException("contract_billing_snapshot.claims_invoice_already_exists");

        if (snapshot.Status == ContractBillingSnapshotStatus.Draft)
            return;

        if (snapshot.Status != ContractBillingSnapshotStatus.PartiallyInvoiced)
            throw new Shared.Exceptions.ValidationException("contract_billing_snapshot.not_open_for_invoicing");
    }

    public static void EnsureInvoiceTypeStillRequired(
        ContractBillingSnapshot snapshot,
        InvoiceType invoiceType,
        IReadOnlyList<Trip> eligibleTrips)
    {
        var slotRequired = invoiceType == InvoiceType.Trips
            ? RequiresTripsInvoice(snapshot)
            : RequiresClaimsInvoice(snapshot);
        var tripRequired = invoiceType == InvoiceType.Trips
            ? RequiresTripsInvoiceForTrips(snapshot, eligibleTrips)
            : RequiresClaimsInvoiceForTrips(snapshot, eligibleTrips);

        if (!slotRequired && !tripRequired)
        {
            throw new Shared.Exceptions.ValidationException(
                "contract_billing_snapshot.invoice_type_not_required_for_snapshot");
        }
    }
}
