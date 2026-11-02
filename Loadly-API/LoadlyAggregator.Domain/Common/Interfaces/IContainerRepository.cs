using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContainerRepository : IRepository<Common.Entities.Container>
{
    Task<Common.Entities.Container?> GetByNameArAsync(string nameAr);

    Task<Common.Entities.Container?> GetByNameEnAsync(string nameEn);

    Task<Common.Entities.Container> GetByIdOrThrowAsync(long id);

    Task<(List<Common.Entities.Container> Containers, int TotalCount)> ListAsync(
        string? search,
        ContainerStatus? status,
        int page,
        int pageSize);
}
