namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleCapacityUnitRepository : IRepository<Common.Entities.VehicleCapacityUnit>
{
    Task<Common.Entities.VehicleCapacityUnit?> GetByCapacityTypeEnAsync(string capacityTypeEn);

    Task<(List<Common.Entities.VehicleCapacityUnit> VehicleCapacityUnits, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);

    Task<Common.Entities.VehicleCapacityUnit> GetByIdOrThrowAsync(long id);
    Task<Dictionary<string, long>> GetIdMapByNamesAsync(List<string> namesLower);
}
