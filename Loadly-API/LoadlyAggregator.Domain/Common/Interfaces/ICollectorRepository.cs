using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ICollectorRepository : IRepository<Common.Entities.Collector>
{
    Task<Common.Entities.Collector?> GetByUserNameAsync(string userName);
    Task<Common.Entities.Collector?> GetByEmailAsync(string email);
    Task<Common.Entities.Collector?> GetByPhoneAsync(string phone);
    Task<Common.Entities.Collector?> GetByLoginAsync(string login);

    Task<Common.Entities.Collector> GetByIdOrThrowAsync(long id);

    Task<bool> ExistsByUserNameAsync(string userName, long excludeId);
    Task<bool> ExistsByEmailAsync(string email, long excludeId);
    Task<bool> ExistsByPhoneAsync(string phone, long excludeId);

    Task<(List<Common.Entities.Collector> Collectors, int TotalCount)> ListAsync(
        string? search,
        int? status,
        long? excludeUserId,
        int page,
        int pageSize);

    Task<int> BulkUpdateStatusAsync(List<long> collectorIds, UserStatus status);

    Task<List<long>> GetExistingIdsAsync(List<long> collectorIds);
}
