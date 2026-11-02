using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilities;

public class DeleteVehicleFacilityAction
{
    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;

    public DeleteVehicleFacilityAction(IVehicleFacilityRepository vehicleFacilityRepository)
    {
        _vehicleFacilityRepository = vehicleFacilityRepository;
    }

    public async Task ExecuteAsync(DeleteVehicleFacilityDto dto)
    {
        _vehicleFacilityRepository.Remove(await _vehicleFacilityRepository.GetByIdOrThrowAsync(dto.Id));
        await _vehicleFacilityRepository.SaveChangesAsync();
    }
}
