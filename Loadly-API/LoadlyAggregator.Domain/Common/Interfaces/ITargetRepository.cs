using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITargetRepository : IRepository<Target>
{
    Task<Target?> GetByMonthYearAndTypeAsync(TargetMonth month, int year, TargetType targetType, long? excludeId = null);
    Task<(List<Target> Targets, int TotalCount)> ListAsync(int? year, TargetType? targetType, int page, int pageSize);
    Task<Target> GetByIdOrThrowAsync(long id);
    Task<IReadOnlyList<Target>> GetMonthlyTargetsAsync(int? year = null, int? month = null);
}
