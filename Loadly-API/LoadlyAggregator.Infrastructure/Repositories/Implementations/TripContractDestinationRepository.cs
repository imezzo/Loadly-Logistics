using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripContractDestinationRepository : Repository<TripContractDestination>, ITripContractDestinationRepository
{
    public TripContractDestinationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<TripContractDestination>> GetByTripIdAsync(long tripId) =>
        await _dbSet.Where(tcd => tcd.TripId == tripId).ToListAsync();

    public async Task<IReadOnlyList<TripContractDestination>> GetByDestinationIdAsync(long destinationId) =>
        await _dbSet.Where(tcd => tcd.DestinationId == destinationId).ToListAsync();
}
