using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripChargeSummaryItemDto
{
    public TripChargeType ChargeType { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalCompanyShare { get; set; }
    public decimal TotalBillable { get; set; }
    public int LineCount { get; set; }
}
