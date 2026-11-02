using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripVehicleAssignmentSnapshotRepository : Repository<TripVehicleAssignmentSnapshot>, ITripVehicleAssignmentSnapshotRepository
{
    public TripVehicleAssignmentSnapshotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task UpsertAsync(
        long tripId,
        string headPlateNo,
        string headPlateLetters,
        string? tailPlateNo,
        string? tailPlateLetters,
        long? addedBy)
    {
        var existing = await _dbSet.FirstOrDefaultAsync(s => s.TripId == tripId);
        if (existing is null)
        {
            await AddAsync(new TripVehicleAssignmentSnapshot
            {
                TripId = tripId,
                HeadPlateNo = headPlateNo,
                HeadPlateLetters = headPlateLetters,
                TailPlateNo = tailPlateNo,
                TailPlateLetters = tailPlateLetters,
                AddedBy = addedBy
            });
            return;
        }

        existing.HeadPlateNo = headPlateNo;
        existing.HeadPlateLetters = headPlateLetters;
        existing.TailPlateNo = tailPlateNo;
        existing.TailPlateLetters = tailPlateLetters;
        Update(existing);
    }

    public async Task RemoveForTripAsync(long tripId)
    {
        var existing = await _dbSet.FirstOrDefaultAsync(s => s.TripId == tripId);
        if (existing is null)
            return;

        Remove(existing);
    }
}
