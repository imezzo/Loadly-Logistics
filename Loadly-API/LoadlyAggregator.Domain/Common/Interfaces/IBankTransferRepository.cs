using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IBankTransferRepository : IRepository<BankTransfer>
{
    Task<BankTransfer> GetByIdOrThrowAsync(long id);

    Task<BankTransfer> GetForAllocationOrThrowAsync(long id);

    Task<IReadOnlyList<BankTransfer>> GetByIdsForAllocationOrThrowAsync(IReadOnlyList<long> ids);

    Task<(List<BankTransfer> Items, int TotalCount)> ListAsync(
        PaymentMethod? paymentMethod,
        string? payerType,
        IReadOnlyList<long>? payerIds,
        string? search,
        int page,
        int pageSize);
}
