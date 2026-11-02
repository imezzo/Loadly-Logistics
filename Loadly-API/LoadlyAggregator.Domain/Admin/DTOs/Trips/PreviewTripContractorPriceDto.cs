namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class PreviewTripContractorPriceDto
{
    public long ContractId { get; set; }
    public long? TripId { get; set; }
    public long? DestinationId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
}
