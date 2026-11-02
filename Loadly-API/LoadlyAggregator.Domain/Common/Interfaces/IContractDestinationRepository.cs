using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractDestinationRepository : IRepository<ContractDestination>
{
    Task<ContractDestination> GetByIdOrThrowAsync(long id);
    Task<List<ContractDestination>> GetByContractIdAsync(long contractId);
    Task<ContractDestination?> GetByContractIdAndWaypointPairAsync(long contractId, long startWaypointId, long endWaypointId);
    Task<Dictionary<long, ContractDestination>> GetMapByContractAndIdsAsync(long contractId, IReadOnlyCollection<long> destinationIds);
}
