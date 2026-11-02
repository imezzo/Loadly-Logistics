using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilityVisits;

public class EnterVehicleFacilityVisitAction
{
    private readonly IVehicleFacilityVisitRepository _visitRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IContractSlotRepository _contractSlotRepository;
    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly VehicleAssignmentHistoryRecorder _assignmentHistoryRecorder;

    public EnterVehicleFacilityVisitAction(
        IVehicleFacilityVisitRepository visitRepository,
        IVehicleRepository vehicleRepository,
        IContractSlotRepository contractSlotRepository,
        IVehicleFacilityRepository vehicleFacilityRepository,
        ITripRepository tripRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        VehicleAssignmentHistoryRecorder assignmentHistoryRecorder)
    {
        _visitRepository = visitRepository;
        _vehicleRepository = vehicleRepository;
        _contractSlotRepository = contractSlotRepository;
        _vehicleFacilityRepository = vehicleFacilityRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _assignmentHistoryRecorder = assignmentHistoryRecorder;
    }

    public async Task ExecuteAsync(EnterVehicleFacilityVisitDto dto)
    {
        var adminId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var vehicle = await _vehicleRepository.GetForFacilityVisitOrThrowAsync(dto.VehicleId);
        var location = await _vehicleFacilityRepository.GetForFacilityVisitOrThrowAsync(dto.VehicleFacilityId);

        VehicleFacilityVisitValidator.ValidateServiceFacilityForEnter(location);

        var coupledVehicle = VehicleFacilityVisitHelper.GetCoupledVehicle(vehicle);

        var vehicles = new List<Vehicle> { vehicle };

        if (dto.IncludeHeadOrTail is not false && coupledVehicle is not null)
            vehicles.Add(coupledVehicle);

        foreach (var v in vehicles)
            VehicleFacilityVisitValidator.ValidateCanEnter(v);

        await VehicleFacilityVisitValidator.ValidateNoInProgressTripsAsync(_tripRepository, vehicles);
        await VehicleFacilityVisitValidator.ValidateEnterEventDatesAsync(
            _visitRepository,
            vehicles.Select(v => v.Id),
            dto.EventDate);

        var headOrRigid = VehicleFacilityVisitHelper.ResolveHeadOrRigid(vehicles);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var visitContextHead = headOrRigid
                ?? (vehicle.HeadVehicleId.HasValue ? coupledVehicle : null);

            var preEnterDriver = headOrRigid?.DriverId;
            var preEnterTail = headOrRigid != null
                ? VehicleAssignmentHistoryRecorder.ResolveTailVehicleId(headOrRigid, vehicles)
                : null;

            Vehicle? decoupledHead = null;
            long? preDecoupledTailId = null;
            if (dto.IncludeHeadOrTail is false && coupledVehicle is not null)
            {
                if (vehicle.HeadVehicleId.HasValue)
                {
                    decoupledHead = coupledVehicle;
                    preDecoupledTailId = vehicle.Id;
                }

                await DecoupleVehicleAsync(vehicle, coupledVehicle);
            }

            var enterVisitDriverByVehicleId = vehicles.ToDictionary(
                v => v.Id,
                v => ResolveEnterVisitDriverId(v, visitContextHead));

            foreach (var v in vehicles)
                await VehicleParkingFacilityHelper.RecordExitAndClearIfAssignedAsync(
                    v, _visitRepository, v.DriverId, adminId);

            long? headContractSlotId = null;
            if (!dto.ClearContract && headOrRigid?.ContractId.HasValue == true)
            {
                headContractSlotId = (await _contractSlotRepository.GetActiveByVehicleIdAsync(headOrRigid.Id))?.Id;
            }

            if (dto.ClearContract)
            {
                foreach (var v in vehicles)
                    await ContractSlotPersistenceHelper.ArchiveActiveSlotForVehicleAsync(v.Id, _contractSlotRepository);
            }

            foreach (var v in vehicles)
                ApplyEnterLocationState(v, dto);

            await _vehicleRepository.UpdateRangeAsync(vehicles);

            await _visitRepository.AddRangeAsync(vehicles.Select(v => new VehicleFacilityVisit
            {
                VehicleId = v.Id,
                VehicleFacilityId = dto.VehicleFacilityId,
                ContractSlotId = headOrRigid != null && v.Id == headOrRigid.Id ? headContractSlotId : null,
                Event = VehicleFacilityVisitEvent.ENTERS_LOCATION,
                Context = VehicleFacilityVisitContext.Service,
                EventDate = dto.EventDate,
                DriverId = enterVisitDriverByVehicleId[v.Id],
                Reason = dto.Reason,
                AddedBy = adminId
            }));

            if (headOrRigid != null)
            {
                await _assignmentHistoryRecorder.RecordChangeAsync(
                    headOrRigid.Id,
                    VehicleAssignmentHistorySource.FacilityEnter,
                    preEnterDriver,
                    preEnterTail,
                    headOrRigid.DriverId,
                    VehicleAssignmentHistoryRecorder.ResolveTailVehicleId(headOrRigid, vehicles),
                    adminId);
                await _tripRepository.UpdateDriverForVehicleTripsAsync(headOrRigid.Id, null, DateTime.UtcNow);
            }
            else if (decoupledHead != null)
            {
                var remainingTails = await _vehicleRepository.GetTailVehiclesByHeadVehicleIdAsync(decoupledHead.Id);
                await _assignmentHistoryRecorder.RecordChangeAsync(
                    decoupledHead.Id,
                    VehicleAssignmentHistorySource.FacilityEnter,
                    decoupledHead.DriverId,
                    preDecoupledTailId,
                    decoupledHead.DriverId,
                    remainingTails.FirstOrDefault()?.Id,
                    adminId);
            }

            await _visitRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static void ApplyEnterLocationState(Vehicle vehicle, EnterVehicleFacilityVisitDto dto)
    {
        vehicle.CurrentServiceFacilityId = dto.VehicleFacilityId;
        vehicle.DriverId = null;
        if (dto.ClearContract)
            vehicle.ContractId = null;
        if (dto.ClearSector)
            vehicle.SectorId = null;
        vehicle.Status = VehicleStatus.OutOfService;
        vehicle.UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateCanEnter(Vehicle vehicle)
    {
        if (vehicle.Status == VehicleStatus.InRoute)
            throw new ValidationException("vehicles.cannot_enter_location_while_in_route");

        if (vehicle.CurrentServiceFacilityId.HasValue || vehicle.Status == VehicleStatus.OutOfService)
            throw new ValidationException("vehicles.already_parked_at_location");
    }

    private async Task ValidateNoInProgressTripsAsync(IEnumerable<Vehicle> vehicles)
    {
        foreach (var v in vehicles)
        {
            if (await _tripRepository.ExistsAsync(t => t.VehicleId == v.Id && t.Status == TripStatus.InProgress))
                throw new ValidationException("vehicles.cannot_enter_location_with_in_progress_trip");
        }
    }

    // Linked navigations don't load VehicleType, so also treat "referenced as a head by a group member" as head.
    private static Vehicle? ResolveHeadOrRigid(List<Vehicle> group) =>
        group.FirstOrDefault(v =>
            v.VehicleType?.VehicleUnitType is VehicleUnitType.Head or VehicleUnitType.RIGID
            || group.Any(x => x.HeadVehicleId == v.Id));

    private static Vehicle? GetCoupledVehicle(Vehicle vehicle) =>
        vehicle.HeadVehicleId.HasValue ? vehicle.HeadVehicle : vehicle.TailVehicles.FirstOrDefault();
    /// <summary>Facility visit context: assigned driver at enter time (tails inherit head driver).</summary>
    private static long? ResolveEnterVisitDriverId(Vehicle vehicle, Vehicle? headOrRigid) =>
        vehicle.DriverId ?? headOrRigid?.DriverId;

    private async Task DecoupleVehicleAsync(Vehicle vehicle, Vehicle coupledVehicle)
    {
        // HeadVehicleId lives on the tail only
        var tail = vehicle.HeadVehicleId.HasValue ? vehicle : coupledVehicle;
        tail.HeadVehicleId = null;
        tail.UpdatedAt = DateTime.UtcNow;

        // Tail not in update list when we entered via head + IncludeHeadOrTail=false
        if (tail.Id != vehicle.Id)
            await _vehicleRepository.UpdateAsync(tail);
    }
}
