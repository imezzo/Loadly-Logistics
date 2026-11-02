using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleFacilityRepository : Repository<VehicleFacility>, IVehicleFacilityRepository
{
    private const string VehicleFacilityModelType = "VehicleFacility";

    public VehicleFacilityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<VehicleFacility?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(v => v.Company!).ThenInclude(c => c.CompanyDetail)
            .Include(v => v.Contacts.Where(c => c.ModelType == VehicleFacilityModelType).OrderBy(c => c.CreatedAt))
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.Vehicle)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.Driver)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.AddedByAdmin)
            .FirstOrDefaultAsync(v => v.Id == id);

    public async Task<VehicleFacility?> GetByNameAsync(string name) =>
        await _dbSet
            .FirstOrDefaultAsync(v => v.Name.ToLower() == name.ToLower());

    public async Task<(List<VehicleFacility> VehicleFacilities, int TotalCount)> ListAsync(
        string? search,
        VehicleFacilityStatus? status,
        VehicleFacilityType? facilityType,
        List<long>? companyIds,
        int page,
        int pageSize)
    {
        var query = _dbSet.AsQueryable()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.Name.Contains(search!) ||
                (v.Address != null && v.Address.Contains(search!)))
            .Where(v => !status.HasValue || v.Status == status.Value)
            .Where(v => !facilityType.HasValue || v.FacilityType == facilityType.Value)
            .Where(v => companyIds == null || companyIds.Count == 0 ||
                (v.CompanyId.HasValue && companyIds.Contains(v.CompanyId.Value)));

        return (await query
                .AsNoTracking()
                .Include(v => v.Company!).ThenInclude(c => c.CompanyDetail)
                .Include(v => v.Contacts.Where(c => c.ModelType == VehicleFacilityModelType).OrderBy(c => c.CreatedAt))
                .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                    .ThenInclude(vv => vv.Vehicle)
                .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                    .ThenInclude(vv => vv.Driver)
                .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                    .ThenInclude(vv => vv.AddedByAdmin)
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            await query.CountAsync());
    }

    public async Task<VehicleFacility> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("vehicle_facilities.not_found_with_id", id);

    public async Task<VehicleFacility> GetForFacilityVisitOrThrowAsync(long id) =>
        await _dbSet.FirstOrDefaultAsync(v => v.Id == id)
        ?? throw new NotFoundException("vehicle_facilities.not_found_with_id", id);

    public async Task<IReadOnlyDictionary<long, string>> GetNamesByIdsAsync(IReadOnlyCollection<long> ids)
    {
        if (ids.Count == 0)
            return new Dictionary<long, string>();

        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(facility => ids.Contains(facility.Id))
            .ToDictionaryAsync(facility => facility.Id, facility => facility.Name);
    }
}
