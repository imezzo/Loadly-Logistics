using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Contractor.Trips;

public class ContractorTripPayoutStatisticsResource
{
    public int CompletedPaidTrips { get; set; }
    public int CompletedUnpaidTrips { get; set; }
    public decimal CompletedPaidTripsTotalAmount { get; set; }
    public decimal CompletedUnpaidTripsTotalAmount { get; set; }

    public static ContractorTripPayoutStatisticsResource FromDto(ContractorTripPayoutStatisticsDto dto) => new()
    {
        CompletedPaidTrips = dto.CompletedPaidTrips,
        CompletedUnpaidTrips = dto.CompletedUnpaidTrips,
        CompletedPaidTripsTotalAmount = dto.CompletedPaidTripsTotalAmount,
        CompletedUnpaidTripsTotalAmount = dto.CompletedUnpaidTripsTotalAmount
    };
}
