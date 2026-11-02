using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleInsurances;

public class CreateVehicleInsuranceAction
{
    private readonly IVehicleInsuranceRepository _vehicleInsuranceRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateVehicleInsuranceAction(
        IVehicleInsuranceRepository vehicleInsuranceRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleInsuranceRepository = vehicleInsuranceRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateVehicleInsuranceDto dto)
    {
        if (dto.EndDate <= dto.StartDate)
            throw new LogicalException("validation.end_date_before_start_date");

        var vehicleInsurance = new Common.Entities.VehicleInsurance
        {
            PolicyNo = dto.PolicyNo,
            InsuranceProvider = dto.InsuranceProvider,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Amount = dto.Amount,
            AddedBy = _currentUserService.GetCurrentUserId()
        };
        await _vehicleInsuranceRepository.AddAsync(vehicleInsurance);
        await _vehicleInsuranceRepository.SaveChangesAsync();
    }
}
