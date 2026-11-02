using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractDestinationRepository : Repository<ContractDestination>, IContractDestinationRepository
{
    public ContractDestinationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<ContractDestination?> GetByIdAsync(long id) =>
        _dbSet
            .Include(cd => cd.Contract)
            .Include(cd => cd.StartWaypointNavigation)
            .Include(cd => cd.EndWaypointNavigation)
            .FirstOrDefaultAsync(cd => cd.Id == id);

    public async Task<ContractDestination> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contracts.contract_destination_not_found_with_id", id);

    public Task<List<ContractDestination>> GetByContractIdAsync(long contractId) =>
        _dbSet
            .Where(cd => cd.ContractId == contractId)
            .ToListAsync();

    public Task<ContractDestination?> GetByContractIdAndWaypointPairAsync(long contractId, long startWaypointId, long endWaypointId) =>
        _dbSet.FirstOrDefaultAsync(cd =>
            cd.ContractId == contractId
            && cd.StartWaypointId == startWaypointId
            && cd.EndWaypointId == endWaypointId);

    public async Task<Dictionary<long, ContractDestination>> GetMapByContractAndIdsAsync(
        long contractId,
        IReadOnlyCollection<long> destinationIds)
    {
        if (destinationIds is null || destinationIds.Count == 0)
            return new Dictionary<long, ContractDestination>();

        var idSet = destinationIds.Where(id => id > 0).Distinct().ToList();
        if (idSet.Count == 0)
            return new Dictionary<long, ContractDestination>();

        var destinations = await _dbSet
            .Include(cd => cd.StartWaypointNavigation)
            .Include(cd => cd.EndWaypointNavigation)
            .Where(cd => cd.ContractId == contractId
                && idSet.Contains(cd.Id)
                && cd.Status != ContractDestinationStatus.ARCHIVED)
            .ToListAsync();

        return destinations.ToDictionary(cd => cd.Id, cd => cd);
    }
}
