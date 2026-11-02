namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleBrandModelRepository : IRepository<Common.Entities.VehicleBrandModel>
{
    Task<Common.Entities.VehicleBrandModel?> GetByNameEnAndBrandIdAsync(string nameEn, long vehicleBrandId);
    Task<(List<Common.Entities.VehicleBrandModel> VehicleBrandModels, int TotalCount)> ListAsync(
        string? search,
        long? vehicleBrandId,
        int page,
        int pageSize);
    Task<Common.Entities.VehicleBrandModel> GetByIdOrThrowAsync(long id);
    Task<Dictionary<string, long>> GetIdMapByNamesAsync(List<string> namesLower);
}
