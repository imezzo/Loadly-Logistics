namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

internal sealed class TripRecordingMetrics
{
    public int WaitingMinutesAtStops { get; init; }
    public int LoadingDurationMinutes { get; init; }
    public int UnloadingDurationMinutes { get; init; }
    public int LoadingUnloadingCompletions { get; init; }
    public decimal BillableTripHours { get; init; }
    public decimal? TollgatesTotalAmount { get; init; }
    public decimal? TollgatesWeightTotalAmount { get; init; }
}
