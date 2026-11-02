using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class BulkUpdateTripChargesInvoiceTypeDto
{
    public IReadOnlyList<long> TripIds { get; set; } = [];
    public IReadOnlyList<TripChargeInvoiceTypeDto> Charges { get; set; } = [];
}

public class TripChargeInvoiceTypeDto
{
    public TripChargeType ChargeType { get; set; }
    public InvoiceType? InvoiceType { get; set; }
}
