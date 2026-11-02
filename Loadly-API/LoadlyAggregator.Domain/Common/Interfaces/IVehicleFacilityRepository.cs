using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleFacilityRepository : IRepository<Common.Entities.VehicleFacility>
{
    Task<Common.Entities.VehicleFacility?> GetByNameAsync(string name);

    Task<(List<Common.Entities.VehicleFacility> VehicleFacilities, int TotalCount)> ListAsync(
        string? search,
        VehicleFacilityStatus? status,
        VehicleFacilityType? facilityType,
        List<long>? companyIds,
        int page,
        int pageSize);

    Task<Common.Entities.VehicleFacility> GetByIdOrThrowAsync(long id);
    Task<Common.Entities.VehicleFacility> GetForFacilityVisitOrThrowAsync(long id);
    Task<IReadOnlyDictionary<long, string>> GetNamesByIdsAsync(IReadOnlyCollection<long> ids);
}
