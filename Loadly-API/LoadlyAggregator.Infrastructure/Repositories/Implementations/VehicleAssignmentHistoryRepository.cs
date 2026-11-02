using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleAssignmentHistoryRepository : Repository<VehicleAssignmentHistory>, IVehicleAssignmentHistoryRepository
{
    public VehicleAssignmentHistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task AddChangeAsync(
        long vehicleId,
        VehicleAssignmentHistorySource source,
        long? previousDriverId,
        long? previousTailVehicleId,
        long? newDriverId,
        long? newTailVehicleId,
        long? addedBy)
    {
        await AddAsync(new VehicleAssignmentHistory
        {
            VehicleId = vehicleId,
            Source = source,
            PrevDriverId = previousDriverId,
            PrevTailVehicleId = previousTailVehicleId,
            NewDriverId = newDriverId,
            NewTailVehicleId = newTailVehicleId,
            AddedBy = addedBy
        });
    }

    public async Task<(List<VehicleAssignmentHistory> Items, int TotalCount)> ListAsync(
        long vehicleId,
        VehicleAssignmentHistorySource? source,
        DateTime? createdFrom,
        DateTime? createdTo,
        int page,
        int pageSize)
    {
        var normalizedPage = page > 0 ? page : 1;
        var normalizedPageSize = pageSize switch
        {
            > 100 => 100,
            > 0 => pageSize,
            _ => 15
        };

        var query = _dbSet
            .AsNoTracking()
            .Include(h => h.PrevDriver)
            .Include(h => h.NewDriver)
            .Include(h => h.PrevTailVehicle)
            .Include(h => h.NewTailVehicle)
            .Include(h => h.AddedByAdmin)
            .Where(h => h.VehicleId == vehicleId)
            .Where(h => !source.HasValue || h.Source == source.Value)
            .Where(h => !createdFrom.HasValue || h.CreatedAt >= createdFrom.Value)
            .Where(h => !createdTo.HasValue || h.CreatedAt <= createdTo.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(h => h.CreatedAt)
            .ThenByDescending(h => h.Id)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
