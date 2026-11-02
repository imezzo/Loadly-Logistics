namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleBrandRepository : IRepository<Common.Entities.VehicleBrand>
{
    Task<Common.Entities.VehicleBrand?> GetByNameEnAsync(string nameEn);
    Task<(List<Common.Entities.VehicleBrand> VehicleBrands, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
    Task<Common.Entities.VehicleBrand> GetByIdOrThrowAsync(long id);
}
