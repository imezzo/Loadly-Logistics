namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class UpdateTripChargesDto
{
    public long Id { get; set; }
    public IReadOnlyList<UpdateTripChargeItemDto> TripCharges { get; set; } = [];
}
