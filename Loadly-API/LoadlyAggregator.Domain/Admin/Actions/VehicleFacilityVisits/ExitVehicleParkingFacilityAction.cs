using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilityVisits;

public class ExitVehicleParkingFacilityAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;
    private readonly IVehicleFacilityVisitRepository _visitRepository;
    private readonly IContractSlotRepository _contractSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ExitVehicleParkingFacilityAction(
        IVehicleRepository vehicleRepository,
        IVehicleFacilityRepository vehicleFacilityRepository,
        IVehicleFacilityVisitRepository visitRepository,
        IContractSlotRepository contractSlotRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleFacilityRepository = vehicleFacilityRepository;
        _visitRepository = visitRepository;
        _contractSlotRepository = contractSlotRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(ExitVehicleParkingFacilityDto dto)
    {
        var adminId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var vehicle = await _vehicleRepository.GetByIdOrThrowAsync(dto.VehicleId);
        VehicleFacilityVisitValidator.ValidateCanExitParking(vehicle, dto.VehicleFacilityId);

        var facilityId = dto.VehicleFacilityId ?? vehicle.AssignedParkingFacilityId!.Value;
        var facility = await _vehicleFacilityRepository.GetByIdOrThrowAsync(facilityId);
        VehicleFacilityVisitValidator.ValidateParkingFacility(facility);

        long? contractSlotId = null;
        if (vehicle.ContractId.HasValue)
        {
            contractSlotId = (await _contractSlotRepository.GetActiveByVehicleIdAsync(vehicle.Id))?.Id;
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await VehicleParkingFacilityHelper.ApplyClearAsync(
                vehicle,
                _visitRepository,
                vehicle.DriverId,
                adminId,
                dto.EventDate,
                dto.Reason,
                contractSlotId);

            _vehicleRepository.Update(vehicle);
            await _visitRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
