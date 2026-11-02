using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IPortRepository : IRepository<Common.Entities.Port>
{
    Task<Common.Entities.Port?> GetByNameArCountryAndTypeAsync(string nameAr, string country, PortType type);

    Task<Common.Entities.Port?> GetByPortCodeAsync(string portCode);

    Task<Common.Entities.Port> GetByIdOrThrowAsync(long id);

    Task<(List<Common.Entities.Port> Ports, int TotalCount)> ListAsync(
        string? search,
        PortType? type,
        PortStatus? status,
        int page,
        int pageSize);
}
