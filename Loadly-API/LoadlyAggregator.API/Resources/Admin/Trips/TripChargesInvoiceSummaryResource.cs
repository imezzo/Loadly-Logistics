using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.API.Resources.Admin.Trips;

public class TripChargesInvoiceSummaryResource
{
    public bool IsRoutingConsistent { get; set; }
    public List<ChargeTypeInvoiceRoutingConflictResource> RoutingConflicts { get; set; } = [];
    public int ChargesCount { get; set; }
    public int ChargesWithInvoiceTypeSet { get; set; }
    public bool AllChargesInvoiceTypeSet { get; set; }
    public List<long> TripInvoiceIds { get; set; } = [];
    public List<long> ClaimInvoiceIds { get; set; } = [];
    public TripChargesInvoiceBucketResource TripInvoice { get; set; } = new();
    public TripChargesInvoiceBucketResource ClaimInvoice { get; set; } = new();
    public decimal GrandTotalBillable { get; set; }

    public static TripChargesInvoiceSummaryResource FromDto(TripChargesInvoiceSummaryDto dto) => new()
    {
        IsRoutingConsistent = dto.IsRoutingConsistent,
        ChargesCount = dto.ChargesCount,
        ChargesWithInvoiceTypeSet = dto.ChargesWithInvoiceTypeSet,
        AllChargesInvoiceTypeSet = dto.AllChargesInvoiceTypeSet,
        GrandTotalBillable = dto.GrandTotalBillable,
        TripInvoiceIds = dto.TripInvoiceIds.ToList(),
        ClaimInvoiceIds = dto.ClaimInvoiceIds.ToList(),
        TripInvoice = TripChargesInvoiceBucketResource.FromDto(dto.TripInvoice),
        ClaimInvoice = TripChargesInvoiceBucketResource.FromDto(dto.ClaimInvoice),
        RoutingConflicts = dto.RoutingConflicts.Select(c => new ChargeTypeInvoiceRoutingConflictResource
        {
            ChargeType = (int)c.ChargeType,
            ChargeTypeName = c.ChargeType.ToLocalizedDisplay(),
            Routes = c.Routes.Select(r => new ChargeTypeInvoiceRouteResource
            {
                InvoiceType = (int)r.InvoiceType,
                TripIds = r.TripIds.ToList()
            }).ToList()
        }).ToList()
    };
}

public class ChargeTypeInvoiceRoutingConflictResource
{
    public int ChargeType { get; set; }
    public string ChargeTypeName { get; set; } = string.Empty;
    public List<ChargeTypeInvoiceRouteResource> Routes { get; set; } = [];
}

public class ChargeTypeInvoiceRouteResource
{
    public int InvoiceType { get; set; }
    public List<long> TripIds { get; set; } = [];
}

public class TripChargesInvoiceBucketResource
{
    public List<TripChargeSummaryItemResource> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public decimal TotalCompanyShare { get; set; }
    public decimal TotalBillable { get; set; }
    public int LineCount { get; set; }

    public static TripChargesInvoiceBucketResource FromDto(TripChargesInvoiceBucketDto dto) => new()
    {
        TotalAmount = dto.TotalAmount,
        TotalCompanyShare = dto.TotalCompanyShare,
        TotalBillable = dto.TotalBillable,
        LineCount = dto.LineCount,
        Items = dto.Items.Select(i => new TripChargeSummaryItemResource
        {
            ChargeType = (int)i.ChargeType,
            ChargeTypeName = i.ChargeType.ToLocalizedDisplay(),
            TotalAmount = i.TotalAmount,
            TotalCompanyShare = i.TotalCompanyShare,
            TotalBillable = i.TotalBillable,
            LineCount = i.LineCount
        }).ToList()
    };
}

public class TripChargeSummaryItemResource
{
    public int ChargeType { get; set; }
    public string ChargeTypeName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal TotalCompanyShare { get; set; }
    public decimal TotalBillable { get; set; }
    public int LineCount { get; set; }
}
