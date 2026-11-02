using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IShippingLineRepository : IRepository<Common.Entities.ShippingLine>
{
    Task<Common.Entities.ShippingLine?> GetByNameArAsync(string nameAr);

    Task<Common.Entities.ShippingLine?> GetByNameEnAsync(string nameEn);

    Task<Common.Entities.ShippingLine> GetByIdOrThrowAsync(long id);

    Task<(List<Common.Entities.ShippingLine> ShippingLines, int TotalCount)> ListAsync(
        string? search,
        ShippingLineStatus? status,
        int page,
        int pageSize);
}
