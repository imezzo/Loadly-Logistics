using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDiscountRepository : IRepository<Discount>
{
    Task<Discount?> GetByNameAsync(string nameEn);

    Task<(List<Discount> Discounts, int TotalCount)> ListAsync(
        string? search,
        DiscountType? type,
        DiscountStatus? status,
        int page,
        int pageSize);

    Task<Discount> GetByIdOrThrowAsync(long id);
}
