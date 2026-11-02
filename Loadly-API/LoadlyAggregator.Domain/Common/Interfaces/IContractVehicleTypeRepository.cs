namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractVehicleTypeRepository : IRepository<Common.Entities.ContractVehicleType>
{
    Task<Common.Entities.ContractVehicleType?> GetByContractIdAndVehicleTypeIdAsync(long contractId, long vehicleTypeId);
    Task<List<Common.Entities.ContractVehicleType>> GetByContractIdAsync(long contractId);
    Task<List<Common.Entities.ContractVehicleType>> GetByContractIdsAsync(IEnumerable<long> contractIds);
    Task<List<Common.Entities.ContractVehicleType>> GetByVehicleTypeIdAsync(long vehicleTypeId);

    Task<(List<Common.Entities.ContractVehicleType> Items, int TotalCount)> ListAsync(
        long? contractId,
        long? vehicleTypeId,
        int page,
        int pageSize);

    Task<Common.Entities.ContractVehicleType> GetByIdOrThrowAsync(long id);
}
