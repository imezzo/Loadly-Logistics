namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractSlotRepository : IRepository<Common.Entities.ContractSlot>
{
    Task<List<Common.Entities.ContractSlot>> GetByContractIdAsync(long contractId);

    Task<Common.Entities.ContractSlot?> GetActiveByVehicleIdAsync(long vehicleId, long? excludeSlotId = null);
}
