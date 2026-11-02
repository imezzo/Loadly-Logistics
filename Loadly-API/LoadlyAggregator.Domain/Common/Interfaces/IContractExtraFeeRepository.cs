namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractExtraFeeRepository : IRepository<Common.Entities.ContractExtraFee>
{
    Task<List<Common.Entities.ContractExtraFee>> GetByContractIdAsync(long contractId);
    Task<List<Common.Entities.ContractExtraFee>> GetByContractIdsAsync(IEnumerable<long> contractIds);
    Task<Common.Entities.ContractExtraFee?> GetByContractIdAndExtraFeeTypeIdAsync(long contractId, int extraFeeTypeId);
    Task<List<Common.Entities.ContractExtraFee>> GetByExtraFeeTypeIdAsync(int extraFeeTypeId);

    Task<(List<Common.Entities.ContractExtraFee> Items, int TotalCount)> ListAsync(
        long? contractId,
        int? extraFeeTypeId,
        int page,
        int pageSize);

    Task<Common.Entities.ContractExtraFee> GetByIdOrThrowAsync(long id);
}
