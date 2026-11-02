using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleInsurances;

public class UpdateVehicleInsuranceAction
{
    private readonly IVehicleInsuranceRepository _vehicleInsuranceRepository;

    public UpdateVehicleInsuranceAction(IVehicleInsuranceRepository vehicleInsuranceRepository)
    {
        _vehicleInsuranceRepository = vehicleInsuranceRepository;
    }

    public async Task ExecuteAsync(UpdateVehicleInsuranceDto dto)
    {
        if (dto.EndDate <= dto.StartDate)
            throw new LogicalException("validation.end_date_before_start_date");

        var vehicleInsurance = await _vehicleInsuranceRepository.GetByIdOrThrowAsync(dto.Id);

        vehicleInsurance.PolicyNo = dto.PolicyNo;
        vehicleInsurance.InsuranceProvider = dto.InsuranceProvider;
        vehicleInsurance.StartDate = dto.StartDate;
        vehicleInsurance.EndDate = dto.EndDate;
        vehicleInsurance.Amount = dto.Amount;
        await _vehicleInsuranceRepository.UpdateAsync(vehicleInsurance);
        await _vehicleInsuranceRepository.SaveChangesAsync();
    }
}
