using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class TripCharge : BaseEntity
{
    public long TripId { get; set; }
    public TripChargeType ChargeType { get; set; }
    public decimal Amount { get; set; }
    public decimal? CompanyShare { get; set; }
    public int? ExtraFeeTypeId { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public string? Notes { get; set; }

    public virtual Trip? Trip { get; set; }
}
