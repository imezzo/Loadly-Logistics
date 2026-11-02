namespace LoadlyAggregator.Domain.Admin.DTOs.Contractors;

public record ContractorAggregateCounts(
    int TotalDrivers,
    int TotalVehicles,
    int TotalTrips,
    int TotalCompletedTrips,
    int TotalCompletedPaidTrips,
    int TotalCompletedUnderReviewTrips,
    int TotalCompletedReadyToPayTrips);
