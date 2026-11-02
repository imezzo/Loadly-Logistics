using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehiclePlateAliasRepository : IRepository<VehiclePlateAlias>
{
    Task AddIfNotExistsAsync(long vehicleId, string plateNo, string plateLetters, long? addedBy);

    Task<Vehicle?> GetVehicleByAliasPlateAsync(string plateNo, string plateLetters);

    Task<IReadOnlyDictionary<(string PlateNo, string PlateLetters), Vehicle>> GetVehicleMapByAliasPlatesAsync(
        IReadOnlyCollection<(string PlateNo, string PlateLetters)> plates);
}
