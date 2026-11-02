using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehiclePlateAliasRepository : Repository<VehiclePlateAlias>, IVehiclePlateAliasRepository
{
    public VehiclePlateAliasRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task AddIfNotExistsAsync(long vehicleId, string plateNo, string plateLetters, long? addedBy)
    {
        var normalizedNo = plateNo.Trim();
        var normalizedLetters = plateLetters.Trim();
        var exists = await _dbSet.AnyAsync(a =>
            a.PlateNo.ToLower() == normalizedNo.ToLower()
            && a.PlateLetters.ToLower() == normalizedLetters.ToLower());
        if (exists)
            return;

        await AddAsync(new VehiclePlateAlias
        {
            VehicleId = vehicleId,
            PlateNo = normalizedNo,
            PlateLetters = normalizedLetters,
            AddedBy = addedBy
        });
    }

    public async Task<Vehicle?> GetVehicleByAliasPlateAsync(string plateNo, string plateLetters)
    {
        var alias = await _dbSet
            .AsNoTracking()
            .Include(a => a.Vehicle!)
                .ThenInclude(v => v.TailVehicles)
            .FirstOrDefaultAsync(a =>
                a.PlateNo.ToLower() == plateNo.ToLower()
                && a.PlateLetters.ToLower() == plateLetters.ToLower());

        return alias?.Vehicle;
    }

    public async Task<IReadOnlyDictionary<(string PlateNo, string PlateLetters), Vehicle>> GetVehicleMapByAliasPlatesAsync(
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

        var plateNoSet = normalized.Select(p => p.PlateNo).Distinct().ToList();
        var plateLettersSet = normalized.Select(p => p.PlateLetters).Distinct().ToList();
        var requested = normalized.ToHashSet();

        var aliases = await _dbSet
            .AsNoTracking()
            .Include(a => a.Vehicle!)
                .ThenInclude(v => v.TailVehicles)
            .Include(a => a.Vehicle!)
                .ThenInclude(v => v.Driver)
            .Where(a => plateNoSet.Contains(a.PlateNo.ToLower())
                     && plateLettersSet.Contains(a.PlateLetters.ToLower()))
            .ToListAsync();

        return aliases
            .Where(a => a.Vehicle != null
                        && requested.Contains((a.PlateNo.Trim().ToLower(), a.PlateLetters.Trim().ToLower())))
            .GroupBy(a => (a.PlateNo.Trim().ToLower(), a.PlateLetters.Trim().ToLower()))
            .ToDictionary(g => g.Key, g => g.First().Vehicle!);
    }
}
