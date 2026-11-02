using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractVehicleTypeRepository : Repository<ContractVehicleType>, IContractVehicleTypeRepository
{
    public ContractVehicleTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<ContractVehicleType?> GetByIdAsync(long id) =>
        _dbSet
            .Include(cvt => cvt.Contract)
            .Include(cvt => cvt.VehicleType)
            .FirstOrDefaultAsync(cvt => cvt.Id == id);

    public Task<ContractVehicleType?> GetByContractIdAndVehicleTypeIdAsync(long contractId, long vehicleTypeId) =>
        _dbSet
            .Include(cvt => cvt.Contract)
            .Include(cvt => cvt.VehicleType)
            .FirstOrDefaultAsync(cvt => cvt.ContractId == contractId && cvt.VehicleTypeId == vehicleTypeId);

    public Task<List<ContractVehicleType>> GetByContractIdAsync(long contractId) =>
        _dbSet
            .Include(cvt => cvt.Contract)
            .Include(cvt => cvt.VehicleType)
            .Where(cvt => cvt.ContractId == contractId)
            .ToListAsync();

    public Task<List<ContractVehicleType>> GetByContractIdsAsync(IEnumerable<long> contractIds) =>
        _dbSet
            .Include(cvt => cvt.Contract)
            .Include(cvt => cvt.VehicleType)
            .Where(cvt => contractIds.Contains(cvt.ContractId))
            .ToListAsync();

    public Task<List<ContractVehicleType>> GetByVehicleTypeIdAsync(long vehicleTypeId) =>
        _dbSet
            .Include(cvt => cvt.Contract)
            .Include(cvt => cvt.VehicleType)
            .Where(cvt => cvt.VehicleTypeId == vehicleTypeId)
            .ToListAsync();

    public override IQueryable<ContractVehicleType> Query() =>
        _dbSet.Include(cvt => cvt.Contract).Include(cvt => cvt.VehicleType).AsQueryable();

    public async Task<(List<ContractVehicleType> Items, int TotalCount)> ListAsync(
        long? contractId,
        long? vehicleTypeId,
        int page,
        int pageSize) =>
        (await _dbSet
            .AsNoTracking()
            .Include(cvt => cvt.Contract)
            .Include(cvt => cvt.VehicleType)
            .Where(cvt => !contractId.HasValue || cvt.ContractId == contractId.Value)
            .Where(cvt => !vehicleTypeId.HasValue || cvt.VehicleTypeId == vehicleTypeId.Value)
            .OrderByDescending(cvt => cvt.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet
            .Where(cvt => !contractId.HasValue || cvt.ContractId == contractId.Value)
            .Where(cvt => !vehicleTypeId.HasValue || cvt.VehicleTypeId == vehicleTypeId.Value)
            .CountAsync());

    public async Task<ContractVehicleType> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contract_vehicle_types.not_found_with_id", id);
}

