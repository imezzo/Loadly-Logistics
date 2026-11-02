using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractSlotRepository : Repository<ContractSlot>, IContractSlotRepository
{
    public ContractSlotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<List<ContractSlot>> GetByContractIdAsync(long contractId) =>
        _dbSet.Where(cs => cs.ContractId == contractId).ToListAsync();

    public Task<ContractSlot?> GetActiveByVehicleIdAsync(long vehicleId, long? excludeSlotId = null) =>
        _dbSet
            .Where(cs =>
                cs.VehicleId == vehicleId
                && cs.Status == ContractSlotStatus.ACTIVE
                && (excludeSlotId == null || cs.Id != excludeSlotId))
            .FirstOrDefaultAsync();
}
