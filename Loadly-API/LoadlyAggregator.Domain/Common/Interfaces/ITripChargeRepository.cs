using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripChargeRepository : IRepository<TripCharge>
{
    Task<IReadOnlyList<(TripChargeType ChargeType, decimal TotalAmount, decimal TotalCompanyShare, int LineCount)>>
        SummarizeByChargeTypeAsync(IReadOnlyCollection<long> tripIds);

    Task<int> BulkUpdateInvoiceTypeByChargeTypeAsync(
        IReadOnlyList<long> tripIds,
        IReadOnlyList<(TripChargeType ChargeType, InvoiceType? InvoiceType)> mappings);
}
