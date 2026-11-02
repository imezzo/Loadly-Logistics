namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class PushTripToContractorsDto
{
    public long TripId { get; set; }
    public long VehicleTypeId { get; set; }
    public decimal? SpotPrice { get; set; }
    public decimal? ContractorPrice { get; set; }
    public bool SendToAllContractors { get; set; }
    public List<long>? ContractorIds { get; set; }
}
