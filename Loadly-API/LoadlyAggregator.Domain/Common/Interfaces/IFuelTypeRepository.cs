namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IFuelTypeRepository : IRepository<Common.Entities.FuelType>
{
    Task<Common.Entities.FuelType?> GetByNameEnAsync(string nameEn);
    Task<Common.Entities.FuelType?> GetByNameArAsync(string nameAr);

    Task<(List<Common.Entities.FuelType> FuelTypes, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
    Task<Common.Entities.FuelType> GetByIdOrThrowAsync(long id);
}
