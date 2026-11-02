using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilityVisits;

public class ExitVehicleFacilityVisitAction
{
    private readonly IVehicleFacilityVisitRepository _visitRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IContractSlotRepository _contractSlotRepository;
    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly ITripRepository _tripRepository;
    private readonly ISectorRepository _sectorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly VehicleAssignmentHistoryRecorder _assignmentHistoryRecorder;

    public ExitVehicleFacilityVisitAction(
        IVehicleFacilityVisitRepository visitRepository,
        IVehicleRepository vehicleRepository,
        IContractSlotRepository contractSlotRepository,
        IVehicleFacilityRepository vehicleFacilityRepository,
        IDriverRepository driverRepository,
        ITripRepository tripRepository,
        ISectorRepository sectorRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        VehicleAssignmentHistoryRecorder assignmentHistoryRecorder)
    {
        _visitRepository = visitRepository;
        _vehicleRepository = vehicleRepository;
        _contractSlotRepository = contractSlotRepository;
        _vehicleFacilityRepository = vehicleFacilityRepository;
        _driverRepository = driverRepository;
        _tripRepository = tripRepository;
        _sectorRepository = sectorRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _assignmentHistoryRecorder = assignmentHistoryRecorder;
    }

    public async Task ExecuteAsync(ExitVehicleFacilityVisitDto dto)
    {
        var adminId = _currentUserService.GetCurrentUserId() ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var vehicle = await _vehicleRepository.GetForFacilityVisitOrThrowAsync(dto.VehicleId);
        var location = await _vehicleFacilityRepository.GetForFacilityVisitOrThrowAsync(dto.VehicleFacilityId);

        _ = await _sectorRepository.GetByIdOrThrowAsync(dto.SectorId);

        VehicleFacilityVisitValidator.ValidateServiceFacilityNotParking(location);

        if (vehicle.VehicleType?.VehicleUnitType == VehicleUnitType.Tail)
        {
            await ExecuteTailExitAsync(dto, vehicle, adminId);
            return;
        }

        await ExecuteHeadOrRigidExitAsync(dto, vehicle, adminId);
    }

    private async Task ExecuteTailExitAsync(ExitVehicleFacilityVisitDto dto, Vehicle tail, long adminId)
    {
        if (dto.IncludeHeadOrTail is false)
            throw new ValidationException("vehicles.tail_cannot_exit_without_head");

        VehicleFacilityVisitValidator.ValidateCanExit(tail, dto.VehicleFacilityId);

        var plan = await ResolveTailExitPlanAsync(tail, dto);
        await ValidateDriverForTailExitAsync(dto.DriverId, plan.Head, plan.DriverAssignedVehicles);

        if (plan.ShouldLinkTailToHead)
            await ValidateHeadTailCapacityAsync(plan.Head, tail.Id);

        var vehiclesToExit = plan.ShouldExitHead ? new List<Vehicle> { tail, plan.Head } : new List<Vehicle> { tail };

        var preExitDriver = plan.Head.DriverId;
        var preExitTail = tail.HeadVehicleId == plan.Head.Id
            ? (long?)tail.Id
            : (await _vehicleRepository.GetTailVehiclesByHeadVehicleIdAsync(plan.Head.Id)).FirstOrDefault()?.Id;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (plan.ShouldLinkTailToHead)
            {
                tail.HeadVehicleId = plan.Head.Id;
                tail.UpdatedAt = DateTime.UtcNow;
            }

            foreach (var v in vehiclesToExit)
            {
                v.CurrentServiceFacilityId = null;
                v.Status = VehicleStatus.Available;
                v.SectorId = dto.SectorId;
                v.UpdatedAt = DateTime.UtcNow;
            }

            plan.Head.DriverId = dto.DriverId;
            plan.Head.UpdatedAt = DateTime.UtcNow;

            long? headContractSlotId = null;
            if (plan.Head.ContractId.HasValue)
            {
                headContractSlotId = (await _contractSlotRepository.GetActiveByVehicleIdAsync(plan.Head.Id))?.Id;
            }

            var vehiclesToUpdate = plan.ShouldExitHead ? vehiclesToExit : new List<Vehicle> { tail, plan.Head };
            await _vehicleRepository.UpdateRangeAsync(vehiclesToUpdate);
            await _visitRepository.AddRangeAsync(vehiclesToExit.Select(v => new VehicleFacilityVisit
            {
                VehicleId = v.Id,
                VehicleFacilityId = dto.VehicleFacilityId,
                ContractSlotId = v.Id == plan.Head.Id ? headContractSlotId : null,
                Event = VehicleFacilityVisitEvent.EXITS_LOCATION,
                Context = VehicleFacilityVisitContext.Service,
                EventDate = dto.EventDate,
                DriverId = dto.DriverId,
                Reason = dto.Reason,
                AddedBy = adminId
            }));

            await _assignmentHistoryRecorder.RecordChangeAsync(
                plan.Head.Id,
                VehicleAssignmentHistorySource.FacilityExit,
                preExitDriver,
                preExitTail,
                plan.Head.DriverId,
                VehicleAssignmentHistoryRecorder.ResolveTailVehicleId(plan.Head, vehiclesToUpdate),
                adminId);
            await _tripRepository.UpdateDriverForVehicleTripsAsync(plan.Head.Id, dto.DriverId, DateTime.UtcNow);

            await _visitRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task ExecuteHeadOrRigidExitAsync(ExitVehicleFacilityVisitDto dto, Vehicle vehicle, long adminId)
    {
        var coupledVehicle = VehicleFacilityVisitHelper.GetCoupledVehicle(vehicle);
        var vehicles = new List<Vehicle> { vehicle };
        if (dto.IncludeHeadOrTail is not false && coupledVehicle is not null)
            vehicles.Add(coupledVehicle);

        foreach (var v in vehicles)
            VehicleFacilityVisitValidator.ValidateCanExit(v, dto.VehicleFacilityId);

        var headOrRigid = VehicleFacilityVisitHelper.ResolveHeadOrRigid(vehicles);
        if (headOrRigid != null)
            await ValidateDriverAssignmentAsync(dto.DriverId, headOrRigid.Id, headOrRigid.ContractorId);
        else
            _ = await _driverRepository.GetForFacilityVisitOrThrowAsync(dto.DriverId);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var preExitDriver = headOrRigid?.DriverId;
            var preExitTail = headOrRigid != null
                ? VehicleAssignmentHistoryRecorder.ResolveTailVehicleId(headOrRigid, vehicles)
                : null;

            if (dto.IncludeHeadOrTail is false && coupledVehicle is not null)
                await DecoupleVehicleAsync(vehicle, coupledVehicle);

            long? headContractSlotId = null;
            if (headOrRigid?.ContractId.HasValue == true)
            {
                headContractSlotId = (await _contractSlotRepository.GetActiveByVehicleIdAsync(headOrRigid.Id))?.Id;
            }

            foreach (var v in vehicles)
            {
                v.CurrentServiceFacilityId = null;
                v.Status = VehicleStatus.Available;
                v.SectorId = dto.SectorId;
                v.UpdatedAt = DateTime.UtcNow;
                if (v.Id == headOrRigid?.Id)
                    v.DriverId = dto.DriverId;
            }

            await _vehicleRepository.UpdateRangeAsync(vehicles);
            await _visitRepository.AddRangeAsync(vehicles.Select(v => new VehicleFacilityVisit
            {
                VehicleId = v.Id,
                VehicleFacilityId = dto.VehicleFacilityId,
                ContractSlotId = headOrRigid != null && v.Id == headOrRigid.Id ? headContractSlotId : null,
                Event = VehicleFacilityVisitEvent.EXITS_LOCATION,
                Context = VehicleFacilityVisitContext.Service,
                EventDate = dto.EventDate,
                DriverId = dto.DriverId,
                Reason = dto.Reason,
                AddedBy = adminId
            }));

            if (headOrRigid != null)
            {
                await _assignmentHistoryRecorder.RecordChangeAsync(
                    headOrRigid.Id,
                    VehicleAssignmentHistorySource.FacilityExit,
                    preExitDriver,
                    preExitTail,
                    headOrRigid.DriverId,
                    VehicleAssignmentHistoryRecorder.ResolveTailVehicleId(headOrRigid, vehicles),
                    adminId);
                await _tripRepository.UpdateDriverForVehicleTripsAsync(headOrRigid.Id, dto.DriverId, DateTime.UtcNow);
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

    private async Task<TailExitPlan> ResolveTailExitPlanAsync(
        Vehicle tail,
        ExitVehicleFacilityVisitDto dto)
    {
        if (tail.HeadVehicleId.HasValue)
        {
            var head = await ResolveLinkedHeadAsync(tail);

            if (head.Status == VehicleStatus.OutOfService)
            {
                if (head.CurrentServiceFacilityId != dto.VehicleFacilityId)
                    throw new ValidationException("vehicles.linked_head_parked_at_different_facility");

                VehicleFacilityVisitValidator.ValidateCanExit(head, dto.VehicleFacilityId);
                return new TailExitPlan(head, ShouldExitHead: true, ShouldLinkTailToHead: false, DriverAssignedVehicles: null);
            }

            if (head.Status == VehicleStatus.Available)
            {
                ValidateHeadAvailableForTailExit(head);
                return new TailExitPlan(head, ShouldExitHead: false, ShouldLinkTailToHead: false, DriverAssignedVehicles: null);
            }

            throw new ValidationException("vehicles.head_must_be_available_for_tail_exit");
        }

        var (headFromDriver, assignedVehicles) = await ResolveHeadViaDriverAsync(dto.DriverId);
        ValidateHeadAvailableForTailExit(headFromDriver);
        return new TailExitPlan(headFromDriver, ShouldExitHead: false, ShouldLinkTailToHead: true, DriverAssignedVehicles: assignedVehicles);
    }

    private sealed record TailExitPlan(
        Vehicle Head,
        bool ShouldExitHead,
        bool ShouldLinkTailToHead,
        IReadOnlyList<Vehicle>? DriverAssignedVehicles);

    private async Task<Vehicle> ResolveLinkedHeadAsync(Vehicle tail)
    {
        var headId = tail.HeadVehicleId!.Value;
        var head = tail.HeadVehicle;
        if (head == null || head.VehicleType == null)
            head = await _vehicleRepository.GetForFacilityVisitOrThrowAsync(headId);

        ValidateIsHead(head);
        return head;
    }

    private async Task<(Vehicle Head, IReadOnlyList<Vehicle> AssignedVehicles)> ResolveHeadViaDriverAsync(long driverId)
    {
        var assignedVehicles = await _vehicleRepository.GetByDriverIdAsync(driverId);
        var head = assignedVehicles.FirstOrDefault(v => v.VehicleType?.VehicleUnitType == VehicleUnitType.Head)
            ?? throw new ValidationException("vehicles.tail_cannot_exit_without_head");

        ValidateIsHead(head);
        return (head, assignedVehicles);
    }

    private async Task ValidateDriverForTailExitAsync(
        long driverId,
        Vehicle head,
        IReadOnlyList<Vehicle>? driverAssignedVehicles)
    {
        var driver = await _driverRepository.GetForFacilityVisitOrThrowAsync(driverId);
        ValidateDriverAvailable(driver);

        if (driverAssignedVehicles != null)
        {
            if (driverAssignedVehicles.Any(v => v.Id != head.Id))
                throw new ValidationException("vehicles.driver_already_assigned_with_id", driverId);
        }
        else if (await _vehicleRepository.IsDriverAssignedToOtherVehicleAsync(driverId, head.Id))
        {
            throw new ValidationException("vehicles.driver_already_assigned_with_id", driverId);
        }

        if (head.ContractorId.HasValue && driver.ContractorId != head.ContractorId)
            throw new ValidationException("vehicles.driver_contractor_mismatch", driverId, head.ContractorId.Value);
    }

    private async Task ValidateHeadTailCapacityAsync(Vehicle head, long tailId)
    {
        var otherTailCount = await _vehicleRepository.CountTailVehiclesByHeadVehicleIdAsync(head.Id, tailId);
        if (otherTailCount >= 1)
            throw new ValidationException("vehicles.head_vehicle_tail_limit_when_adding", head.Id, otherTailCount, otherTailCount + 1);
    }

    private static void ValidateIsHead(Vehicle head)
    {
        if (head.VehicleType?.VehicleUnitType != VehicleUnitType.Head)
            throw new ValidationException("vehicles.head_vehicle_is_not_head", head.Id);
    }

    private static void ValidateHeadAvailableForTailExit(Vehicle head)
    {
        if (head.Status != VehicleStatus.Available)
            throw new ValidationException("vehicles.head_must_be_available_for_tail_exit");
    }

    private static void ValidateDriverAvailable(Common.Entities.Driver driver)
    {
        if ((driver.WorkingStatus ?? DriverWorkingStatus.Offline) != DriverWorkingStatus.Available)
            throw new ValidationException("vehicles.driver_must_be_available_for_tail_exit");
    }

    private async Task DecoupleVehicleAsync(Vehicle vehicle, Vehicle coupledVehicle)
    {
        if (vehicle.HeadVehicleId.HasValue)
        {
            vehicle.HeadVehicleId = null;
            vehicle.UpdatedAt = DateTime.UtcNow;
            return;
        }

        coupledVehicle.HeadVehicleId = null;
        coupledVehicle.UpdatedAt = DateTime.UtcNow;
        await _vehicleRepository.UpdateAsync(coupledVehicle);
    }

    private async Task ValidateDriverAssignmentAsync(long driverId, long vehicleId, long? contractorId)
    {
        var driver = await _driverRepository.GetForFacilityVisitOrThrowAsync(driverId);
        await ValidateDriverAssignmentAsync(driver, driverId, vehicleId, contractorId);
    }

    private async Task ValidateDriverAssignmentAsync(
        Common.Entities.Driver driver,
        long driverId,
        long vehicleId,
        long? contractorId)
    {
        if (await _vehicleRepository.IsDriverAssignedToOtherVehicleAsync(driverId, vehicleId))
            throw new ValidationException("vehicles.driver_already_assigned_with_id", driverId);

        if (contractorId.HasValue && driver.ContractorId != contractorId)
            throw new ValidationException("vehicles.driver_contractor_mismatch", driverId, contractorId.Value);
    }
}
