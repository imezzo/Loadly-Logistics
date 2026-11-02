using System.Collections.Generic;
using System.Linq;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IAddressRepository : IRepository<Address>
{
    Task<IEnumerable<Address>> GetByModelAsync(string modelType, long modelId);
    Task<List<Address>> GetByModelAndModelIdsAsync(string modelType, IEnumerable<long> modelIds);
    Task<(IEnumerable<Address> Addresses, int TotalCount)> SearchAsync(
        string? search = null,
        string? modelType = null,
        long? modelId = null,
        long? governerateId = null,
        long? cityId = null,
        long? districtId = null,
        int page = 1,
        int pageSize = 10);

    Task<(List<Address> Addresses, int TotalCount)> ListAsync(
        string? search,
        string? modelType,
        long? modelId,
        long? governerateId,
        long? cityId,
        long? districtId,
        int page,
        int pageSize);

    Task<Address> GetByIdOrThrowAsync(long id);
}

