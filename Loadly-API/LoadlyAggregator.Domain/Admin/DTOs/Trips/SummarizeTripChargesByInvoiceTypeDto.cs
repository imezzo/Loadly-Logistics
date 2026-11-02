using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class SummarizeTripChargesByInvoiceTypeDto
{
    public IReadOnlyList<long> TripIds { get; set; } = [];
}

public class ChargeTypeInvoiceRouteDto
{
    public InvoiceType InvoiceType { get; set; }
    public IReadOnlyList<long> TripIds { get; set; } = [];
}

public class ChargeTypeInvoiceRoutingConflictDto
{
    public TripChargeType ChargeType { get; set; }
    public IReadOnlyList<ChargeTypeInvoiceRouteDto> Routes { get; set; } = [];
}

public class TripChargesInvoiceBucketDto
{
    public List<TripChargeSummaryItemDto> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public decimal TotalCompanyShare { get; set; }
    public decimal TotalBillable { get; set; }
    public int LineCount { get; set; }
}

public class TripChargesInvoiceSummaryDto
{
    public bool IsRoutingConsistent { get; set; }
    public IReadOnlyList<ChargeTypeInvoiceRoutingConflictDto> RoutingConflicts { get; set; } = [];
    public int ChargesCount { get; set; }
    public int ChargesWithInvoiceTypeSet { get; set; }
    public bool AllChargesInvoiceTypeSet { get; set; }
    public IReadOnlyList<long> TripInvoiceIds { get; set; } = [];
    public IReadOnlyList<long> ClaimInvoiceIds { get; set; } = [];
    public TripChargesInvoiceBucketDto TripInvoice { get; set; } = new();
    public TripChargesInvoiceBucketDto ClaimInvoice { get; set; } = new();
    public decimal GrandTotalBillable { get; set; }
}
