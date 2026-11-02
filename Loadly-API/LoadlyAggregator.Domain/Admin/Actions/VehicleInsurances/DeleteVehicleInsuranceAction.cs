using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleInsurances;

public class DeleteVehicleInsuranceAction
{
    private readonly IVehicleInsuranceRepository _vehicleInsuranceRepository;

    public DeleteVehicleInsuranceAction(IVehicleInsuranceRepository vehicleInsuranceRepository)
    {
        _vehicleInsuranceRepository = vehicleInsuranceRepository;
    }

    public async Task ExecuteAsync(DeleteVehicleInsuranceDto dto)
    {
        var vehicleInsurance = await _vehicleInsuranceRepository.GetByIdOrThrowAsync(dto.Id);
        _vehicleInsuranceRepository.Remove(vehicleInsurance);
        await _vehicleInsuranceRepository.SaveChangesAsync();
    }
}
