using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITaxRepository : IRepository<Common.Entities.Tax>
{
    Task<Common.Entities.Tax?> GetByNameAsync(string nameEn);

    Task<(List<Common.Entities.Tax> Taxes, int TotalCount)> ListAsync(
        string? search,
        TaxType? type,
        TaxStatus? status,
        int page,
        int pageSize);

    Task<Common.Entities.Tax> GetByIdOrThrowAsync(long id);
}
