namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class UpdateTripChargeItemDto
{
    public long Id { get; set; }
    public int ChargeType { get; set; }
    public int? ExtraFeeTypeId { get; set; }
    public decimal Amount { get; set; }
    public decimal? CompanyShare { get; set; }
}
