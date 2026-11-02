using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleRepository : IRepository<Common.Entities.Vehicle>
{
    Task<Common.Entities.Vehicle?> GetByPlateNoAndLettersAsync(string plateNo, string plateLetters);
    Task<Common.Entities.Vehicle?> GetByVinNoAsync(string vinNo);
    Task<Common.Entities.Vehicle> GetByIdOrThrowAsync(long id);
    Task<Common.Entities.Vehicle> GetForFacilityVisitOrThrowAsync(long id);
    Task<Common.Entities.Vehicle> GetByHeadIdAsyn(long HeadVehicleId);
    Task<bool> IsDriverAssignedAsync(long driverId);
    Task<bool> IsDriverAssignedToOtherVehicleAsync(long driverId, long currentVehicleId);
    Task<bool> IsHeadVehicleAssignedAsync(long headVehicleId);
    Task<bool> IsHeadVehicleAssignedToOtherVehicleAsync(long headVehicleId, long currentVehicleId);
    Task<List<Common.Entities.Vehicle>> GetByContractIdAsync(long contractId);
    Task<List<Common.Entities.Vehicle>> GetByDriverIdAsync(long driverId);
    Task UpdateStatusAsync(long vehicleId, VehicleStatus status);
    Task<List<Common.Entities.Vehicle>> GetTailVehiclesByHeadVehicleIdAsync(long headVehicleId);
    Task<int> CountTailVehiclesByHeadVehicleIdAsync(long headVehicleId, long? excludeTailId = null);
    Task ClearContractAssignmentByContractIdAsync(long contractId);
    Task<List<Common.Entities.Vehicle>> GetVehiclesByIdsAsync(List<long> vehicleIds);
    Task UpdateTailVehiclesHeadAssignmentAsync(List<long> tailVehicleIds, long? headVehicleId);
    Task<(List<Common.Entities.Vehicle> Vehicles, int TotalCount)> ListAsync(
        string? search = null,
        long? contractId = null,
        bool? isAssignedToContract = null,
        long? colorId = null,
        long? capacityUnitId = null,
        long? vehicleTypeId = null,
        List<long>? vehicleTypeIds = null,
        List<long>? headLookupForTailTypeIds = null,
        VehicleUnitType? VehicleUnitType = null,
        List<VehicleTypeCategory>? vehicleTypeCategories = null,
        int? status = null,
        bool? isAvailableTail = null,
        long? driverId = null,
        List<long>? contractorIds = null,
        bool? hasContractor = null,
        bool? hasDriver = null,
        List<long>? sectorIds = null,
        List<long>? companyIds = null,
        List<long>? facilityIds = null,
        List<VehicleFacilityType>? facilityTypes = null,
        int? page = null,
        int? pageSize = null);

    Task<List<string>> GetExistingVinNosAsync(List<string> vinNos);

    Task<Dictionary<(string PlateNo, string PlateLetters), Common.Entities.Vehicle>> GetMapByPlatesAsync(
        IReadOnlyCollection<(string PlateNo, string PlateLetters)> plates);

    Task<(List<Common.Entities.Vehicle> Vehicles, int TotalCount)> ListWithChildrenAsync(
        string? search = null,
        long? contractId = null,
        bool? isAssignedToContract = null,
        long? colorId = null,
        long? capacityUnitId = null,
        long? vehicleTypeId = null,
        List<long>? vehicleTypeIds = null,
        List<long>? headLookupForTailTypeIds = null,
        VehicleUnitType? vehicleUnitType = null,
        List<VehicleTypeCategory>? vehicleTypeCategories = null,
        int? status = null,
        bool? isAvailableTail = null,
        long? driverId = null,
        List<long>? contractorIds = null,
        bool? hasContractor = null,
        bool? hasDriver = null,
        List<long>? sectorIds = null,
        List<long>? companyIds = null,
        List<long>? facilityIds = null,
        List<VehicleFacilityType>? facilityTypes = null,
        bool? orderByCompletedTripsDescending = null,
        bool? isWaitingForLoading = null,
        int page = 1,
        int pageSize = 10);

    Task<IReadOnlyDictionary<VehicleStatus, int>> GetVehicleStatusCountsAsync();

    Task<IReadOnlyDictionary<VehicleFacilityType, int>> GetParkedVehicleFacilityTypeCountsAsync();

    Task<IReadOnlyList<(long SectorId, int Total, int Active)>> GetVehicleCountsBySectorAsync();
}
