using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleTypeRepository : Repository<VehicleType>, IVehicleTypeRepository
{
    private readonly DbSet<VehicleTypeTag> _vehicleTypeTags;

    public VehicleTypeRepository(ApplicationDbContext context) : base(context)
    {
        _vehicleTypeTags = context.Set<VehicleTypeTag>();
    }

    public override async Task<VehicleType?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(v => v.VehicleTypeTags)
            .ThenInclude(vtt => vtt.Tag)
            .FirstOrDefaultAsync(v => v.Id == id);

    public async Task<VehicleType?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(v => v.NameEn.ToLower() == nameEn.ToLower());

    public async Task<(List<VehicleType> VehicleTypes, int TotalCount)> ListAsync(
        string? search = null,
        VehicleTypeCategory? category = null,
        List<VehicleUnitType>? vehicleUnitTypes = null,
        VehicleTypeStatus? status = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(v => string.IsNullOrWhiteSpace(search) || v.NameEn.Contains(search!) || (v.NameAr != null && v.NameAr.Contains(search!)))
            .Where(v => !category.HasValue || v.Category == category.Value)
            .Where(v => vehicleUnitTypes == null || !vehicleUnitTypes.Any() || (v.VehicleUnitType.HasValue && vehicleUnitTypes.Contains(v.VehicleUnitType.Value)))
            .Where(v => !status.HasValue || v.Status == status.Value);

        var totalCount = await query.CountAsync();
        var vehicleTypes = await query
            .Include(v => v.VehicleTypeTags)
            .ThenInclude(vtt => vtt.Tag)
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (vehicleTypes, totalCount);
    }

    public async Task<VehicleType> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("vehicle_types.not_found_with_id", id);

    public async Task<Dictionary<(VehicleTypeCategory Category, string Name), VehicleType>> GetMapByCategoryAndNamesAsync(
        IEnumerable<(VehicleTypeCategory Category, string Name)> keys)
    {
        var keyList = keys.Distinct().ToList();
        var categories = keyList.Select(k => k.Category).Distinct().ToList();
        var names = keyList.Select(k => k.Name).Distinct().ToList();

        var vehicleTypes = await _dbSet
            .Where(v => v.Status == VehicleTypeStatus.ACTIVE
                        && categories.Contains(v.Category)
                        && names.Contains(v.NameEn.ToLower()))
            .ToListAsync();

        var keySet = keyList.ToHashSet();

        return vehicleTypes
            .Where(v => keySet.Contains((v.Category, v.NameEn.ToLower())))
            .GroupBy(v => (v.Category, v.NameEn.ToLower()))
            .ToDictionary(g => g.Key, g => g.First());
    }

    public async Task<List<VehicleTypeTag>> GetTagsByVehicleTypeIdAsync(long vehicleTypeId) =>
        await _vehicleTypeTags
            .Where(vtt => vtt.VehicleTypeId == vehicleTypeId)
            .ToListAsync();

    public async Task<List<VehicleTypeTag>> GetLinksByTagIdAsync(long tagId) =>
        await _vehicleTypeTags
            .Where(vtt => vtt.TagId == tagId)
            .ToListAsync();

    public async Task<List<Tag>> GetTagsByIdsAsync(IEnumerable<long> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return [];
        }

        return await _context.Set<Tag>()
            .Where(t => idList.Contains(t.Id))
            .ToListAsync();
    }

    public async Task AddVehicleTypeTagAsync(VehicleTypeTag vehicleTypeTag) =>
        await _vehicleTypeTags.AddAsync(vehicleTypeTag);

    public void RemoveVehicleTypeTags(IEnumerable<VehicleTypeTag> vehicleTypeTags)
    {
        var tags = vehicleTypeTags.ToList();
        foreach (var tag in tags)
        {
            tag.SetDeletedAt(DateTime.UtcNow);
        }

        _vehicleTypeTags.UpdateRange(tags);
    }
}
