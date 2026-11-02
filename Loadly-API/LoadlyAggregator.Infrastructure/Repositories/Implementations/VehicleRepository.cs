using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleRepository(ApplicationDbContext context) : Repository<Vehicle>(context), IVehicleRepository
{
    public override async Task<Vehicle?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(v => v.Color)
            .Include(v => v.CapacityUnit)
            .Include(v => v.Insurance)
            .Include(v => v.VehicleType)
            .Include(v => v.Driver)
            .Include(v => v.CurrentServiceFacility!).ThenInclude(f => f.Company!).ThenInclude(c => c.CompanyDetail)
            .Include(v => v.AssignedParkingFacility!).ThenInclude(f => f.Company!).ThenInclude(c => c.CompanyDetail)
            .Include(v => v.Contractor)
            .Include(v => v.Sector)
            .Include(v => v.HeadVehicle!).ThenInclude(h => h.VehicleType)
            .Include(v => v.TailVehicles)
            .Include(v => v.VehicleBrandModel)
            .Include(v => v.VehicleDetail)
            .Include(v => v.FuelType)
            .Include(v => v.Contract!).ThenInclude(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(v => v.VehicleDocuments).ThenInclude(vd => vd.Document)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.VehicleFacility!).ThenInclude(f => f.Company!).ThenInclude(c => c.CompanyDetail)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.Driver)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.AddedByAdmin)
            .Include(v => v.Trips)
            .Include(v => v.PlateAliases.OrderByDescending(a => a.CreatedAt))
            .AsSingleQuery()
            .FirstOrDefaultAsync(v => v.Id == id);

    public async Task<Vehicle?> GetByPlateNoAndLettersAsync(string plateNo, string plateLetters)
    {
        var vehicle = await _dbSet
            .FirstOrDefaultAsync(v =>
                v.PlateNo.ToLower() == plateNo.ToLower() &&
                v.PlateLetters.ToLower() == plateLetters.ToLower());
        if (vehicle != null)
            return vehicle;

        var alias = await _context.Set<VehiclePlateAlias>()
            .AsNoTracking()
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a =>
                a.PlateNo.ToLower() == plateNo.ToLower()
                && a.PlateLetters.ToLower() == plateLetters.ToLower());

        return alias?.Vehicle;
    }

    public async Task<Vehicle?> GetByVinNoAsync(string vinNo) =>
        await _dbSet
            .FirstOrDefaultAsync(v => vinNo != null && v.VinNo != null && v.VinNo.ToLower() == vinNo.ToLower());

    public async Task<Vehicle> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("vehicles.not_found_with_id", id);

    public async Task<Vehicle> GetForFacilityVisitOrThrowAsync(long id) =>
        await _dbSet
            .Include(v => v.VehicleType)
            .Include(v => v.HeadVehicle!).ThenInclude(h => h.VehicleType)
            .Include(v => v.TailVehicles)
            .FirstOrDefaultAsync(v => v.Id == id)
        ?? throw new NotFoundException("vehicles.not_found_with_id", id);

    public async Task<Vehicle?> GetByHeadIdAsyn(long HeadVehicleId) =>
        await _dbSet
            .FirstOrDefaultAsync(v => v.HeadVehicleId == HeadVehicleId);

    public async Task<bool> IsDriverAssignedAsync(long driverId) =>
        await _dbSet.AnyAsync(v => v.DriverId == driverId);

    public async Task<bool> IsDriverAssignedToOtherVehicleAsync(long driverId, long currentVehicleId) =>
        await _dbSet.AnyAsync(v => v.DriverId == driverId && v.Id != currentVehicleId);

    public async Task<bool> IsHeadVehicleAssignedAsync(long headVehicleId) =>
        await _dbSet.AnyAsync(v => v.HeadVehicleId == headVehicleId);

    public async Task<bool> IsHeadVehicleAssignedToOtherVehicleAsync(long headVehicleId, long currentVehicleId) =>
        await _dbSet.AnyAsync(v => v.HeadVehicleId == headVehicleId && v.Id != currentVehicleId);

    public async Task<List<Vehicle>> GetByContractIdAsync(long contractId) =>
        await _dbSet
            .Include(v => v.VehicleType)
            .Include(v => v.Color)
            .Include(v => v.TailVehicles)
            .Where(v => v.ContractId == contractId)
            .ToListAsync();

    public async Task<List<Vehicle>> GetByDriverIdAsync(long driverId) =>
        await _dbSet
            .Include(v => v.VehicleType)
            .Include(v => v.Color)
            .Where(v => v.DriverId == driverId)
            .ToListAsync();

    public async Task UpdateStatusAsync(long vehicleId, VehicleStatus status)
    {
        var now = DateTime.UtcNow;
        await _dbSet
            .Where(v => v.Id == vehicleId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(v => v.Status, status)
                .SetProperty(v => v.UpdatedAt, now));
    }

    public async Task<List<Vehicle>> GetTailVehiclesByHeadVehicleIdAsync(long headVehicleId) =>
        await _dbSet
            .Where(v => v.HeadVehicleId == headVehicleId)
            .ToListAsync();

    public async Task<int> CountTailVehiclesByHeadVehicleIdAsync(long headVehicleId, long? excludeTailId = null) =>
        await _dbSet.CountAsync(v =>
            v.HeadVehicleId == headVehicleId
            && (!excludeTailId.HasValue || v.Id != excludeTailId.Value));

    public async Task ClearContractAssignmentByContractIdAsync(long contractId)
    {
        var now = DateTime.UtcNow;
        await _dbSet
            .Where(v => v.ContractId == contractId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(v => v.ContractId, (long?)null)
                .SetProperty(v => v.UpdatedAt, now));
    }

    public async Task<List<Vehicle>> GetVehiclesByIdsAsync(List<long> vehicleIds) =>
        await _dbSet
            .Include(v => v.VehicleType)
            .Where(v => vehicleIds.Contains(v.Id))
            .ToListAsync();

    public async Task UpdateTailVehiclesHeadAssignmentAsync(List<long> tailVehicleIds, long? headVehicleId)
    {
        var now = DateTime.UtcNow;
        await _dbSet
            .Where(v => tailVehicleIds.Contains(v.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(v => v.HeadVehicleId, headVehicleId)
                .SetProperty(v => v.UpdatedAt, now));
    }

    private IQueryable<Vehicle> ApplyListFilters(
        IQueryable<Vehicle> query,
        string? search,
        long? contractId,
        bool? isAssignedToContract,
        long? colorId,
        long? capacityUnitId,
        long? vehicleTypeId,
        List<long>? vehicleTypeIds,
        List<long>? headLookupForTailTypeIds,
        VehicleUnitType? vehicleUnitType,
        List<VehicleTypeCategory>? vehicleTypeCategories,
        int? status,
        bool? isAvailableTail = null,
        long? driverId = null,
        List<long>? contractorIds = null,
        bool? hasContractor = null,
        bool? hasDriver = null,
        List<long>? sectorIds = null,
        List<long>? companyIds = null,
        bool? isWaitingForLoading = null,
        List<long>? facilityIds = null,
        List<VehicleFacilityType>? facilityTypes = null)
    {
        bool hasVehicleTypeFilter = vehicleTypeIds is { Count: > 0 };
        bool hasHeadLookupFilter = headLookupForTailTypeIds is { Count: > 0 };
        bool hasVehicleTypeCategoriesFilter = vehicleTypeCategories is { Count: > 0 };
        bool hasFacilityTypesFilter = facilityTypes is { Count: > 0 };

        return query
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.PlateNo.Contains(search!) ||
                v.PlateLetters.Contains(search!) ||
                //(v.VinNo != null && v.VinNo.Contains(search!)) ||
                (v.Driver != null &&
                    (v.Driver.FullName.Contains(search!) ||
                     v.Driver.FullNameAr.Contains(search!))))
            .Where(v => !contractId.HasValue ||
                v.ContractId == contractId.Value ||
                _context.Contracts.Any(c =>
                    c.Id == contractId.Value &&
                    c.RenewedFromContractId != null &&
                    v.ContractId == c.RenewedFromContractId))
            .Where(v => !colorId.HasValue || v.ColorId == colorId.Value)
            .Where(v => !capacityUnitId.HasValue || v.CapacityUnitId == capacityUnitId.Value)
            .Where(v =>
                (!hasVehicleTypeFilter && !hasHeadLookupFilter)
                || (hasVehicleTypeFilter && vehicleTypeIds!.Contains(v.VehicleTypeId))
                || (hasHeadLookupFilter
                    && v.VehicleType.VehicleUnitType == VehicleUnitType.Head
                    && _dbSet.Any(tail =>
                        tail.HeadVehicleId == v.Id && headLookupForTailTypeIds!.Contains(tail.VehicleTypeId))))
            .Where(v => !vehicleUnitType.HasValue || v.VehicleType.VehicleUnitType == vehicleUnitType.Value)
            .Where(v => !hasVehicleTypeCategoriesFilter || vehicleTypeCategories!.Contains(v.VehicleType.Category))
            .Where(v => !isAvailableTail.HasValue ||
            (isAvailableTail.Value
                ? v.VehicleType.VehicleUnitType == VehicleUnitType.Tail && v.HeadVehicleId == null
                : v.VehicleType.VehicleUnitType == VehicleUnitType.Tail && v.HeadVehicleId != null))
            .Where(v => !status.HasValue || (int)v.Status == status.Value)
            .Where(v => !driverId.HasValue || v.DriverId == driverId.Value)
            .Where(v => contractorIds == null || contractorIds.Count == 0 ||
                (v.ContractorId.HasValue && contractorIds.Contains(v.ContractorId.Value)))
            .Where(v => !hasContractor.HasValue ||
                (hasContractor.Value ? v.ContractorId != null : v.ContractorId == null))
            .Where(v => !hasDriver.HasValue || (hasDriver.Value ? v.DriverId != null : v.DriverId == null))
            .Where(v => sectorIds == null || sectorIds.Count == 0 ||
                (v.SectorId.HasValue && sectorIds.Contains(v.SectorId.Value)))
            .Where(v => companyIds == null || companyIds.Count == 0 ||
                (v.ContractId != null && v.Contract != null && companyIds.Contains(v.Contract.CompanyId)))
            .Where(v => facilityIds == null || facilityIds.Count == 0 ||
                (v.CurrentServiceFacilityId.HasValue && facilityIds.Contains(v.CurrentServiceFacilityId.Value)) ||
                (v.AssignedParkingFacilityId.HasValue && facilityIds.Contains(v.AssignedParkingFacilityId.Value)))
            .Where(v => !hasFacilityTypesFilter ||
                (v.CurrentServiceFacility != null && facilityTypes!.Contains(v.CurrentServiceFacility.FacilityType)) ||
                (v.AssignedParkingFacility != null && facilityTypes.Contains(v.AssignedParkingFacility.FacilityType)))
            .Where(v =>
                !isAssignedToContract.HasValue ||
                (isAssignedToContract.Value && v.ContractId != null) ||
                (!isAssignedToContract.Value && v.ContractId == null))
            .Where(v => !isWaitingForLoading.HasValue ||
                (isWaitingForLoading.Value
                    ? v.AssignedParkingFacilityId != null
                    : v.AssignedParkingFacilityId == null));
    }

    public async Task<(List<Vehicle> Vehicles, int TotalCount)> ListAsync(
        string? search = null,
        long? contractId = null,
        bool? isAssignedToContract = null,
        long? colorId = null,
        long? capacityUnitId = null,
        long? vehicleTypeId = null,
        List<long>? vehicleTypeIds = null,
        List<long>? headLookupForTailTypeIds = null,
        VehicleUnitType? VehicleUnitType = null,
        List<VehicleTypeCategory>? vehicleTypeCategories = null,
        int? status = null,
        bool? isAvailableTail = null,
        long? driverId = null,
        List<long>? contractorIds = null,
        bool? hasContractor = null,
        bool? hasDriver = null,
        List<long>? sectorIds = null,
        List<long>? companyIds = null,
        List<long>? facilityIds = null,
        List<VehicleFacilityType>? facilityTypes = null,
        int? page = null,
        int? pageSize = null)
    {
        var filtered = ApplyListFilters(
            _dbSet.AsQueryable(),
            search,
            contractId,
            isAssignedToContract,
            colorId,
            capacityUnitId,
            vehicleTypeId,
            vehicleTypeIds,
            headLookupForTailTypeIds,
            VehicleUnitType,
            vehicleTypeCategories,
            status,
            isAvailableTail,
            driverId,
                        contractorIds,
            hasContractor,
            hasDriver,
            sectorIds,
            companyIds,
            isWaitingForLoading: null,
            facilityIds,
            facilityTypes);

        IQueryable<Vehicle> withIncludes = filtered
            .AsNoTracking()
            .Include(v => v.Driver)
            .Include(v => v.Contractor)
            .Include(v => v.Sector)
            .Include(v => v.Color)
            .Include(v => v.VehicleType)
            .Include(v => v.FuelType)
            .Include(v => v.Insurance)
            .Include(v => v.CapacityUnit)
            .Include(v => v.HeadVehicle!).ThenInclude(h => h.VehicleType)
            .Include(v => v.TailVehicles)
            .Include(v => v.VehicleBrandModel)
                .ThenInclude(bm => bm!.VehicleBrand)
            .Include(v => v.PlateAliases.OrderByDescending(a => a.CreatedAt))
            .OrderByDescending(v => v.CreatedAt);

        if (page is null || pageSize is null)
        {
            var allVehicles = await withIncludes.ToListAsync();
            return (allVehicles, 0);
        }

        int totalCount = await filtered.CountAsync();
        var vehicles = await withIncludes
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();
        return (vehicles, totalCount);
    }

    public async Task<(List<Vehicle> Vehicles, int TotalCount)> ListWithChildrenAsync(
        string? search = null,
        long? contractId = null,
        bool? isAssignedToContract = null,
        long? colorId = null,
        long? capacityUnitId = null,
        long? vehicleTypeId = null,
        List<long>? vehicleTypeIds = null,
        List<long>? headLookupForTailTypeIds = null,
        VehicleUnitType? vehicleUnitType = null,
        List<VehicleTypeCategory>? vehicleTypeCategories = null,
        int? status = null,
        bool? isAvailableTail = null,
        long? driverId = null,
        List<long>? contractorIds = null,
        bool? hasContractor = null,
        bool? hasDriver = null,
        List<long>? sectorIds = null,
        List<long>? companyIds = null,
        List<long>? facilityIds = null,
        List<VehicleFacilityType>? facilityTypes = null,
        bool? orderByCompletedTripsDescending = null,
        bool? isWaitingForLoading = null,
        int page = 1,
        int pageSize = 10)
    {
        var filtered = ApplyListFilters(
            _dbSet.AsQueryable(),
            search,
            contractId,
            isAssignedToContract,
            colorId,
            capacityUnitId,
            vehicleTypeId,
            vehicleTypeIds,
            headLookupForTailTypeIds,
            vehicleUnitType,
            vehicleTypeCategories,
            status,
            isAvailableTail,
            driverId,
                        contractorIds,
            hasContractor,
            hasDriver,
            sectorIds,
            companyIds,
            isWaitingForLoading,
            facilityIds,
            facilityTypes);

        IQueryable<Vehicle> ordered = orderByCompletedTripsDescending == true
            ? filtered.OrderByDescending(v => v.Trips.Count(t =>
                t.Status == TripStatus.Completed))
            : filtered.OrderByDescending(v => v.CreatedAt);
        var totalCount = await ordered.CountAsync();
        var vehicles = await ordered
            .AsNoTracking()
            .Include(v => v.Driver)
            .Include(v => v.Contractor)
            .Include(v => v.Sector)
            .Include(v => v.Color)
            .Include(v => v.VehicleType)
            .Include(v => v.Insurance)
            .Include(v => v.CapacityUnit)
            .Include(v => v.HeadVehicle!).ThenInclude(h => h.VehicleType)
            .Include(v => v.TailVehicles)
            .Include(v => v.VehicleBrandModel)
            .Include(v => v.VehicleDetail)
            .Include(v => v.FuelType)
            .Include(v => v.Contract!).ThenInclude(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(v => v.Trips)
            .Include(v => v.VehicleDocuments).ThenInclude(vd => vd.Document)
            .Include(v => v.CurrentServiceFacility)
            .Include(v => v.AssignedParkingFacility)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.VehicleFacility!).ThenInclude(f => f.Company!).ThenInclude(c => c.CompanyDetail)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.Driver)
            .Include(v => v.VehicleFacilityVisits.OrderByDescending(vv => vv.EventDate).ThenByDescending(vv => vv.Id))
                .ThenInclude(vv => vv.AddedByAdmin)
            .Include(v => v.PlateAliases.OrderByDescending(a => a.CreatedAt))
            .AsSingleQuery()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (vehicles, totalCount);
    }

    public async Task<List<string>> GetExistingVinNosAsync(List<string> vinNos)
    {
        var lowerVins = vinNos.Select(v => v.Trim().ToLower()).ToList();
        return await _dbSet
            .Where(v => v.VinNo != null && lowerVins.Contains(v.VinNo.ToLower()))
            .Select(v => v.VinNo!)
            .ToListAsync();
    }

    public async Task<Dictionary<(string PlateNo, string PlateLetters), Vehicle>> GetMapByPlatesAsync(
        IReadOnlyCollection<(string PlateNo, string PlateLetters)> plates)
    {
        if (plates is null || plates.Count == 0)
            return new Dictionary<(string PlateNo, string PlateLetters), Vehicle>();

        var normalized = plates
            .Where(p => !string.IsNullOrWhiteSpace(p.PlateNo) && !string.IsNullOrWhiteSpace(p.PlateLetters))
            .Select(p => (PlateNo: p.PlateNo.Trim().ToLower(), PlateLetters: p.PlateLetters.Trim().ToLower()))
            .ToList();

        if (normalized.Count == 0)
            return new Dictionary<(string PlateNo, string PlateLetters), Vehicle>();

        // Narrow the SQL query by the (small) cross of distinct plate numbers and letters,
        // then filter to the exact requested tuples in memory.
        var plateNoSet = normalized.Select(p => p.PlateNo).Distinct().ToList();
        var plateLettersSet = normalized.Select(p => p.PlateLetters).Distinct().ToList();
        var requested = normalized.ToHashSet();

        var vehicles = await _dbSet
            .Include(v => v.TailVehicles)
            .Include(v => v.Driver)
            .Where(v => plateNoSet.Contains(v.PlateNo.ToLower())
                     && plateLettersSet.Contains(v.PlateLetters.ToLower()))
            .ToListAsync();

        var map = vehicles
            .Where(v => requested.Contains((v.PlateNo.Trim().ToLower(), v.PlateLetters.Trim().ToLower())))
            .GroupBy(v => (v.PlateNo.Trim().ToLower(), v.PlateLetters.Trim().ToLower()))
            .ToDictionary(g => g.Key, g => g.First());

        var missingKeys = requested.Where(k => !map.ContainsKey(k)).ToList();
        if (missingKeys.Count == 0)
            return map;

        var aliases = await _context.Set<VehiclePlateAlias>()
            .AsNoTracking()
            .Include(a => a.Vehicle!)
                .ThenInclude(v => v.TailVehicles)
            .Include(a => a.Vehicle!)
                .ThenInclude(v => v.Driver)
            .Where(a => plateNoSet.Contains(a.PlateNo.ToLower())
                     && plateLettersSet.Contains(a.PlateLetters.ToLower()))
            .ToListAsync();

        foreach (var alias in aliases)
        {
            if (alias.Vehicle is null)
                continue;

            var key = (alias.PlateNo.Trim().ToLower(), alias.PlateLetters.Trim().ToLower());
            if (!requested.Contains(key) || map.ContainsKey(key))
                continue;

            map[key] = alias.Vehicle;
        }

        return map;
    }

    public async Task<IReadOnlyDictionary<VehicleStatus, int>> GetVehicleStatusCountsAsync()
    {
        var rows = await _dbSet
            .GroupBy(v => v.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows.ToDictionary(x => x.Status, x => x.Count);
    }

    public async Task<IReadOnlyDictionary<VehicleFacilityType, int>> GetParkedVehicleFacilityTypeCountsAsync()
    {
        var rows = await _dbSet
            .Where(v => v.CurrentServiceFacilityId != null)
            .GroupBy(v => v.CurrentServiceFacility!.FacilityType)
            .Select(g => new { FacilityType = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows.ToDictionary(x => x.FacilityType, x => x.Count);
    }

    public async Task<IReadOnlyList<(long SectorId, int Total, int Active)>> GetVehicleCountsBySectorAsync()
    {
        var rows = await _dbSet
            .Where(v => v.SectorId != null)
            .GroupBy(v => v.SectorId!.Value)
            .Select(g => new
            {
                SectorId = g.Key,
                Total = g.Count(),
                Active = g.Count(v => v.Status == VehicleStatus.InRoute)
            })
            .ToListAsync();

        return rows.Select(r => (r.SectorId, r.Total, r.Active)).ToList();
    }

    public override IQueryable<Vehicle> Query() =>
        base.Query()
            .Include(v => v.HeadVehicle!).ThenInclude(h => h.VehicleType)
            .Include(v => v.TailVehicles);
}
