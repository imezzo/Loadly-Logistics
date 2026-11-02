using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripContractDestinationHelper
{
    public static async Task<ContractDestination?> GetOptionalContractDestinationForTripAsync(
        long contractId,
        long? destinationId,
        IContractDestinationRepository repository)
    {
        if (destinationId is null or <= 0)
            return null;

        ContractDestination? row = await repository.GetByIdAsync(destinationId.Value);
        if (row == null)
            throw new NotFoundException("contracts.contract_destination_not_found_with_id", destinationId.Value);

        if (row.ContractId != contractId)
            throw new ValidationException(TripMessageKeys.ContractDestinationInvalid);

        if (row.Status == ContractDestinationStatus.ARCHIVED)
            throw new ValidationException(TripMessageKeys.ContractDestinationInvalid);

        return row;
    }

    public static async Task<long?> ResolveOptionalDestinationIdAsync(
        long contractId,
        long? destinationId,
        IContractDestinationRepository repository) =>
        (await GetOptionalContractDestinationForTripAsync(contractId, destinationId, repository))?.Id;

    public static int ResolveNumberOfDays(DateTime? startDate, DateTime? endDate, int? destinationNumberOfDays)
    {
        if (destinationNumberOfDays is int days)
            return days;

        return DeriveInclusiveDaySpanFromTripDates(startDate, endDate);
    }

    public static int ResolveNumberOfDays(Trip trip, ContractDestination? destination) =>
        ResolveNumberOfDays(trip.StartDate, trip.EndDate, destination?.NumberOfDays);

    public static void ApplyNumberOfDays(Trip trip, ContractDestination? destination) =>
        trip.NumberOfDays = ResolveNumberOfDays(trip, destination);

    public static IReadOnlyList<TripContractDestination> GetActiveAssignments(Trip trip)
    {
        TripContractDestination? assignment = GetCurrentAssignment(trip);
        return assignment != null ? [assignment] : [];
    }

    public static TripContractDestination? GetCurrentAssignment(Trip trip) =>
        trip.ContractDestinationAssignment is { DeletedAt: null } assignment ? assignment : null;

    public static ContractDestination? GetCurrentContractDestination(Trip trip) =>
        GetCurrentAssignment(trip)?.Destination;

    public static long? GetCurrentContractDestinationId(Trip trip) =>
        GetCurrentAssignment(trip)?.DestinationId;

    public static long? GetCurrentDestinationId(IEnumerable<TripContractDestination> assignments) =>
        assignments
            .Where(tcd => tcd.DeletedAt == null)
            .OrderByDescending(tcd => tcd.Id)
            .FirstOrDefault()
            ?.DestinationId;

    public static decimal? ResolveReturnDistanceKm(
        TripType tripType,
        ContractDestination outbound,
        ContractDestination? returnDestination) =>
        tripType switch
        {
            TripType.RoundTrip => outbound.ReturnExpectedKms,
            TripType.OpenJaw => returnDestination?.ExpectedKms,
            _ => null
        };

    public static void ApplyContractDestinationAssignment(
        TripContractDestination assignment,
        TripType tripType,
        ContractDestination outbound,
        ContractDestination? returnDestination)
    {
        if (tripType == TripType.OpenJaw && returnDestination == null)
            throw new ValidationException(TripMessageKeys.OpenJawReturnDestinationRequired);

        assignment.DestinationId = outbound.Id;
        assignment.OutboundDistanceKm = outbound.ExpectedKms ?? 0;
        assignment.ReturnDestinationId = tripType == TripType.OpenJaw ? returnDestination!.Id : null;
        assignment.ReturnDistanceKm = ResolveReturnDistanceKm(tripType, outbound, returnDestination);
    }

    private static int DeriveInclusiveDaySpanFromTripDates(DateTime? startDate, DateTime? endDate)
    {
        if (startDate is DateTime s && endDate is DateTime e)
            return Math.Max(1, (int)(e.Date - s.Date).TotalDays);

        return 1;
    }
}
