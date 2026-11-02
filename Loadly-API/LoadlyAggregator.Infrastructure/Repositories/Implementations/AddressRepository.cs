using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Address?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<Address>> GetByModelAsync(string modelType, long modelId) =>
        await _dbSet
            .Where(a => a.ModelType == modelType && a.ModelId == modelId)
            .ToListAsync();

    public async Task<List<Address>> GetByModelAndModelIdsAsync(string modelType, IEnumerable<long> modelIds)
    {
        var ids = modelIds.ToList();
        if (ids.Count == 0)
            return new List<Address>();
        return await _dbSet
            .Where(a => a.ModelType == modelType && ids.Contains(a.ModelId))
            .ToListAsync();
    }

    public async Task<(IEnumerable<Address> Addresses, int TotalCount)> SearchAsync(
        string? search = null,
        string? modelType = null,
        long? modelId = null,
        long? governerateId = null,
        long? cityId = null,
        long? districtId = null,
        int page = 1,
        int pageSize = 10) =>
        (await BuildSearchQuery(search, modelType, modelId, governerateId, cityId, districtId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await BuildSearchQuery(search, modelType, modelId, governerateId, cityId, districtId).CountAsync());

    private IQueryable<Address> BuildSearchQuery(string? search, string? modelType, long? modelId, long? governerateId, long? cityId, long? districtId) =>
        _dbSet.AsQueryable()
            .Where(a => string.IsNullOrWhiteSpace(search) || (a.StreetName != null && a.StreetName.Contains(search)) || (a.BuildingNumber != null && a.BuildingNumber.Contains(search)) || (a.PostalCode != null && a.PostalCode.Contains(search)))
            .Where(a => string.IsNullOrWhiteSpace(modelType) || a.ModelType == modelType)
            .Where(a => !modelId.HasValue || a.ModelId == modelId.Value)
            .Where(a => !governerateId.HasValue || a.GovernerateId == governerateId.Value)
            .Where(a => !cityId.HasValue || a.CityId == cityId.Value)
            .Where(a => !districtId.HasValue || a.DistrictId == districtId.Value);

    public async Task<(List<Address> Addresses, int TotalCount)> ListAsync(
        string? search,
        string? modelType,
        long? modelId,
        long? governerateId,
        long? cityId,
        long? districtId,
        int page,
        int pageSize) =>
        (await BuildSearchQuery(search, modelType, modelId, governerateId, cityId, districtId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await BuildSearchQuery(search, modelType, modelId, governerateId, cityId, districtId).CountAsync());

    public async Task<Address> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("addresses.not_found_with_id", id);
}
