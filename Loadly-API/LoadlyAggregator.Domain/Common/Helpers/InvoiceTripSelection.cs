using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class InvoiceTripSelection
{
    private const int MaxTripIdsInErrorMessage = 30;

    public static List<Trip> ResolveEligibleTrips(
        IReadOnlyList<long> tripIds,
        IReadOnlyDictionary<long, Trip> fetchedTrips,
        InvoiceType invoiceType,
        bool isLeadBilling,
        long? contractId,
        long? leadId,
        long? existingInvoiceId = null)
    {
        var trips = new List<Trip>();
        var ineligibleTripIds = new List<long>();

        foreach (var tripId in tripIds)
        {
            if (!fetchedTrips.TryGetValue(tripId, out Trip? trip))
                throw new NotFoundException("trips.not_found_with_id", tripId);

            var linkedInvoiceId = invoiceType == InvoiceType.Trips ? trip.InvoiceId : trip.ClaimInvoiceId;
            if (linkedInvoiceId.HasValue && linkedInvoiceId != existingInvoiceId)
                throw new ValidationException("invoices.trip_already_invoiced", tripId);

            if (!IsEligibleForInvoicing(trip, isLeadBilling, contractId, leadId))
                ineligibleTripIds.Add(tripId);
            else
                trips.Add(trip);
        }

        if (ineligibleTripIds.Count > 0)
        {
            throw new ValidationException(
                "invoices.trip_ids_not_eligible_for_invoice",
                FormatTripIdsForMessage(ineligibleTripIds));
        }

        return trips;
    }

    public static List<Trip> ResolveIncludedTripsInvoiceLinks(
        IReadOnlyList<long> tripIds,
        IReadOnlyDictionary<long, Trip> fetchedTrips,
        long contractId,
        long? existingInvoiceId = null) =>
        ResolveEligibleTrips(
            tripIds,
            fetchedTrips,
            InvoiceType.Trips,
            isLeadBilling: false,
            contractId,
            leadId: null,
            existingInvoiceId);

    public static string FormatTripIdsForMessage(IReadOnlyList<long> tripIds)
    {
        if (tripIds.Count == 0)
            return string.Empty;

        if (tripIds.Count <= MaxTripIdsInErrorMessage)
            return string.Join(", ", tripIds);

        var shown = string.Join(", ", tripIds.Take(MaxTripIdsInErrorMessage));
        return $"{shown} (+{tripIds.Count - MaxTripIdsInErrorMessage} more)";
    }

    private static bool IsEligibleForInvoicing(
        Trip trip,
        bool isLeadBilling,
        long? contractId,
        long? leadId) =>
        trip.Status == TripStatus.Completed
        && (isLeadBilling
            ? trip.LeadId == leadId && trip.IsExternalTrip
            : trip.ContractId == contractId && trip.Contract != null);
}
