using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripContractDistanceHelper
{
    public static decimal? ResolveContractDistanceKm(
        TripType tripType,
        ContractDestination? destination,
        decimal? submittedContractDistanceKm,
        ContractDestination? returnDestination = null)
    {
        if (submittedContractDistanceKm.HasValue)
            return submittedContractDistanceKm;

        if (destination == null)
            return null;

        return CalculateFromDestination(tripType, destination, returnDestination);
    }

    public static decimal? CalculateFromDestination(
        TripType tripType,
        ContractDestination destination,
        ContractDestination? returnDestination = null) =>
        tripType switch
        {
            TripType.OneWay => destination.ExpectedKms,
            TripType.RoundTrip => SumExpectedLegs(destination.ExpectedKms, destination.ReturnExpectedKms),
            TripType.OpenJaw => returnDestination != null
                ? SumExpectedLegs(destination.ExpectedKms, returnDestination.ExpectedKms)
                : destination.ExpectedKms,
            _ => null
        };

    private static decimal? SumExpectedLegs(decimal? outboundKm, decimal? returnKm)
    {
        if (outboundKm is null && returnKm is null)
            return null;

        return (outboundKm ?? 0m) + (returnKm ?? 0m);
    }
}
