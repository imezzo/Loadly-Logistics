namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripChargeDto
{
    public long Id { get; set; }
    public int ChargeType { get; set; }
    public string ChargeTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? CompanyShare { get; set; }
    public int? ExtraFeeTypeId { get; set; }
    public string? Notes { get; set; }
    public TripChargeExtraFeeTypeDto? ExtraFeeType { get; set; }
}
