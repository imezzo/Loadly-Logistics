using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractorPayoutRepository : IRepository<ContractorPayout>
{
    Task<ContractorPayout> GetByIdOrThrowAsync(long id);

    Task<(List<ContractorPayout> Payouts, int TotalCount)> ListAsync(
        long? contractorId,
        ContractorPayoutStatus? status,
        int page,
        int pageSize);

    Task<int?> GetMaxPayoutSequenceForYearAsync(int year);

    Task<int> AllocateNextPayoutSequenceForYearAsync(int year);
}
