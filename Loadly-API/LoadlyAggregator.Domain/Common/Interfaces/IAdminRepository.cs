using System.Linq;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IAdminRepository : IRepository<Common.Entities.Admin>
{
    Task<Common.Entities.Admin?> GetByUserNameAsync(string userName);
    Task<Common.Entities.Admin?> GetByEmailAsync(string email);
    Task<Common.Entities.Admin?> GetByPhoneAsync(string phone);
    Task<Common.Entities.Admin?> GetByLoginAsync(string login);

    Task<Common.Entities.Admin> GetByIdOrThrowAsync(long id);

    Task<bool> ExistsByUserNameAsync(string userName, long excludeId);
    Task<bool> ExistsByEmailAsync(string email, long excludeId);
    Task<bool> ExistsByPhoneAsync(string phone, long excludeId);

    Task<(List<Common.Entities.Admin> Admins, int TotalCount)> ListAsync(
        string? search,
        int? type,
        int? status,
        long? excludeUserId,
        int page,
        int pageSize,
        List<long>? roleIds = null);

    Task<int> BulkUpdateStatusAsync(List<long> adminIds, UserStatus status);

    Task<List<long>> GetExistingIdsAsync(List<long> adminIds);
}

