using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractorRepository : IRepository<Common.Entities.Contractor>
{
    Task<Common.Entities.Contractor?> GetByUserNameAsync(string userName);
    Task<Common.Entities.Contractor?> GetByEmailAsync(string email);
    Task<Common.Entities.Contractor?> GetByPhoneAsync(string phone);
    Task<Common.Entities.Contractor?> GetBySecondPhoneNumberAsync(string secondPhoneNumber);
    Task<Common.Entities.Contractor?> GetByLoginAsync(string login);
    Task<Common.Entities.Contractor?> GetByNationalIdAsync(string nationalId);

    Task<Common.Entities.Contractor> GetByIdOrThrowAsync(long id);

    Task<Common.Entities.Contractor> GetByIdWithChildrenOrThrowAsync(long id);

    Task<ContractorAggregateCounts> GetAggregateCountsAsync(long contractorId);

    Task<Dictionary<long, ContractorAggregateCounts>> GetAggregateCountsByIdsAsync(
        IReadOnlyList<long> contractorIds,
        DateTime? tripStartDate = null,
        DateTime? tripEndDate = null);

    Task<bool> ExistsByUserNameAsync(string userName, long excludeId);
    Task<bool> ExistsByEmailAsync(string email, long excludeId);
    Task<bool> ExistsByPhoneAsync(string phone, long excludeId);
    Task<bool> ExistsBySecondPhoneNumberAsync(string secondPhoneNumber, long excludeId);
    Task<bool> ExistsByNationalIdAsync(string nationalId, long excludeId);

    Task<(List<Common.Entities.Contractor> Contractors, int TotalCount)> ListWithChildrenAsync(
        string? search,
        ContractorStatus? contractorStatus,
        UserStatus? status,
        int page,
        int pageSize,
        DateTime? tripStartDate = null,
        DateTime? tripEndDate = null);
}
