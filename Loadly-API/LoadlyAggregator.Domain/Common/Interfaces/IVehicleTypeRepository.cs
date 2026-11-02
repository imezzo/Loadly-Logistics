using System.Collections.Generic;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleTypeRepository : IRepository<Common.Entities.VehicleType>
{
    Task<Common.Entities.VehicleType?> GetByNameAsync(string nameEn);
    Task<(List<Common.Entities.VehicleType> VehicleTypes, int TotalCount)> ListAsync(
        string? search = null,
        VehicleTypeCategory? category = null,
        List<VehicleUnitType>? vehicleUnitTypes = null,
        VehicleTypeStatus? status = null,
        int page = 1,
        int pageSize = 10);
    Task<Common.Entities.VehicleType> GetByIdOrThrowAsync(long id);
    Task<Dictionary<(VehicleTypeCategory Category, string Name), Common.Entities.VehicleType>> GetMapByCategoryAndNamesAsync(
        IEnumerable<(VehicleTypeCategory Category, string Name)> keys);
    Task<List<Common.Entities.VehicleTypeTag>> GetTagsByVehicleTypeIdAsync(long vehicleTypeId);
    Task<List<Common.Entities.VehicleTypeTag>> GetLinksByTagIdAsync(long tagId);
    Task<List<Common.Entities.Tag>> GetTagsByIdsAsync(IEnumerable<long> ids);
    Task AddVehicleTypeTagAsync(Common.Entities.VehicleTypeTag vehicleTypeTag);
    void RemoveVehicleTypeTags(IEnumerable<Common.Entities.VehicleTypeTag> vehicleTypeTags);
}
