using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class SummarizeTripChargesByInvoiceTypeAction
{
    private readonly ITripRepository _tripRepository;

    public SummarizeTripChargesByInvoiceTypeAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<TripChargesInvoiceSummaryDto> ExecuteAsync(SummarizeTripChargesByInvoiceTypeDto dto)
    {
        var tripIds = dto.TripIds.Distinct().ToList();
        var trips = await _tripRepository.GetByIdsAsync(tripIds);
        var allCharges = trips.SelectMany(t => t.TripCharges ?? []).ToList();
        var conflicts = TripChargeInvoiceRouting.DetectRoutingConflicts(allCharges);

        var tripInvoice = BuildBucket(allCharges, InvoiceType.Trips);
        var claimInvoice = BuildBucket(allCharges, InvoiceType.Claims);
        int chargesCount = allCharges.Count;
        int chargesWithInvoiceTypeSet = allCharges.Count(c => c.InvoiceType.HasValue);

        return new TripChargesInvoiceSummaryDto
        {
            IsRoutingConsistent = conflicts.Count == 0,
            RoutingConflicts = conflicts.Select(c => new ChargeTypeInvoiceRoutingConflictDto
            {
                ChargeType = c.ChargeType,
                Routes = c.Routes.Select(r => new ChargeTypeInvoiceRouteDto
                {
                    InvoiceType = r.InvoiceType,
                    TripIds = r.TripIds
                }).ToList()
            }).ToList(),
            ChargesCount = chargesCount,
            ChargesWithInvoiceTypeSet = chargesWithInvoiceTypeSet,
            AllChargesInvoiceTypeSet = chargesCount > 0 && chargesWithInvoiceTypeSet == chargesCount,
            TripInvoiceIds = trips
                .Where(t => t.InvoiceId.HasValue)
                .Select(t => t.InvoiceId!.Value)
                .Distinct()
                .OrderBy(id => id)
                .ToList(),
            ClaimInvoiceIds = trips
                .Where(t => t.ClaimInvoiceId.HasValue)
                .Select(t => t.ClaimInvoiceId!.Value)
                .Distinct()
                .OrderBy(id => id)
                .ToList(),
            TripInvoice = tripInvoice,
            ClaimInvoice = claimInvoice,
            GrandTotalBillable = tripInvoice.TotalBillable + claimInvoice.TotalBillable
        };
    }

    private static TripChargesInvoiceBucketDto BuildBucket(IEnumerable<TripCharge> charges, InvoiceType invoiceType)
    {
        var filtered = charges
            .Where(c => TripChargeInvoiceRouting.GetEffectiveInvoiceType(c) == invoiceType)
            .ToList();

        var items = filtered
            .GroupBy(c => c.ChargeType)
            .Select(g => new TripChargeSummaryItemDto
            {
                ChargeType = g.Key,
                TotalAmount = g.Sum(c => c.Amount),
                TotalCompanyShare = g.Sum(c => c.CompanyShare ?? 0m),
                TotalBillable = g.Sum(TripChargeTotals.BillableAmount),
                LineCount = g.Count()
            })
            .OrderBy(i => i.ChargeType)
            .ToList();

        return new TripChargesInvoiceBucketDto
        {
            Items = items,
            TotalAmount = filtered.Sum(c => c.Amount),
            TotalCompanyShare = filtered.Sum(c => c.CompanyShare ?? 0m),
            TotalBillable = filtered.Sum(TripChargeTotals.BillableAmount),
            LineCount = filtered.Count
        };
    }
}
