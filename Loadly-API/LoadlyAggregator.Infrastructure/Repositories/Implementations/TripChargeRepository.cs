using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripChargeRepository : Repository<TripCharge>, ITripChargeRepository
{
    public TripChargeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<(TripChargeType ChargeType, decimal TotalAmount, decimal TotalCompanyShare, int LineCount)>>
        SummarizeByChargeTypeAsync(IReadOnlyCollection<long> tripIds)
    {
        var rows = await Query()
            .Where(c => tripIds.Contains(c.TripId))
            .GroupBy(c => c.ChargeType)
            .Select(g => new
            {
                ChargeType = g.Key,
                TotalAmount = g.Sum(c => c.Amount),
                TotalCompanyShare = g.Sum(c => c.CompanyShare ?? 0m),
                LineCount = g.Count()
            })
            .ToListAsync();

        return rows
            .Select(r => (r.ChargeType, r.TotalAmount, r.TotalCompanyShare, r.LineCount))
            .ToList();
    }

    // One UPDATE per distinct invoice type (typically 1–2). Upgrade to CASE/WHEN if groups grow.
    public async Task<int> BulkUpdateInvoiceTypeByChargeTypeAsync(
        IReadOnlyList<long> tripIds,
        IReadOnlyList<(TripChargeType ChargeType, InvoiceType? InvoiceType)> mappings)
    {
        var now = DateTime.UtcNow;
        var total = 0;

        foreach (var group in mappings.GroupBy(m => m.InvoiceType))
        {
            var chargeTypes = group.Select(m => m.ChargeType).ToList();
            var invoiceType = group.Key;

            total += await Query()
                .Where(c => tripIds.Contains(c.TripId) && chargeTypes.Contains(c.ChargeType))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.InvoiceType, invoiceType)
                    .SetProperty(c => c.UpdatedAt, now));
        }

        return total;
    }
}
