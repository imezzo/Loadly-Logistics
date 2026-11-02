using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Domain.Common.Notifications.Channels;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class AssignVehicleDriverAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly VehicleAssignmentHistoryRecorder _assignmentHistoryRecorder;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public AssignVehicleDriverAction(
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        ITripRepository tripRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        VehicleAssignmentHistoryRecorder assignmentHistoryRecorder,
        INotificationEventDispatcher notificationDispatcher)
    {
        _vehicleRepository = vehicleRepository;
        _driverRepository = driverRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _assignmentHistoryRecorder = assignmentHistoryRecorder;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(AssignVehicleDriverDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdOrThrowAsync(dto.VehicleId);

        if (vehicle.VehicleType?.VehicleUnitType == VehicleUnitType.Tail)
            throw new LogicalException("vehicles.cannot_assign_driver_to_tail");

        if (vehicle.DriverId == dto.DriverId)
            return;

        if (await _tripRepository.ExistsAsync(t => t.VehicleId == vehicle.Id && t.Status == TripStatus.InProgress))
            throw new ValidationException("vehicles.cannot_change_driver_with_in_progress_trip");

        if (dto.DriverId.HasValue)
        {
            var driver = await _driverRepository.GetByIdOrThrowAsync(dto.DriverId.Value);

            if (await _vehicleRepository.IsDriverAssignedToOtherVehicleAsync(dto.DriverId.Value, vehicle.Id))
                throw new ValidationException("vehicles.driver_already_assigned_with_id", dto.DriverId.Value);

            if (vehicle.ContractorId.HasValue && driver.ContractorId != vehicle.ContractorId)
                throw new ValidationException("vehicles.driver_contractor_mismatch", dto.DriverId.Value, vehicle.ContractorId.Value);

            if (await _tripRepository.ExistsAsync(t => t.DriverId == dto.DriverId.Value && t.Status == TripStatus.InProgress))
                throw new ValidationException(TripMessageKeys.DriverHasInProgressTrip);
        }

        List<TripScheduledNotificationData>? cascadeNotifications = null;
        long? previousDriverId = vehicle.DriverId;
        long? tailVehicleId = vehicle.TailVehicles.FirstOrDefault()?.Id;
        long? addedBy = _currentUserService.GetCurrentUserId();

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            vehicle.DriverId = dto.DriverId;

            _vehicleRepository.Update(vehicle);

            await _assignmentHistoryRecorder.RecordChangeAsync(
                vehicle.Id,
                VehicleAssignmentHistorySource.AssignDriver,
                previousDriverId,
                tailVehicleId,
                dto.DriverId,
                tailVehicleId,
                addedBy);

            var statusesToUpdate = new[] { TripStatus.Pending, TripStatus.Scheduled };

            DateTime now = DateTime.UtcNow;

            var affectedTrips = await _tripRepository.GetTripsByVehicleAndStatusesAsync(vehicle.Id, statusesToUpdate, now);

            await _tripRepository.UpdateDriverForVehicleTripsAsync(vehicle.Id, dto.DriverId, now);

            cascadeNotifications = affectedTrips
                .Where(t => t.Status == TripStatus.Scheduled && dto.DriverId.HasValue)
                .Select(trip => new TripScheduledNotificationData
                {
                    TripId = trip.Id,
                    CompanyId = trip.Contract?.CompanyId,
                    DriverId = dto.DriverId!.Value,
                    DriverPushKind = TripScheduleDriverPushKind.VehicleCascadeReassignment
                })
                .ToList();

            await _vehicleRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        if (cascadeNotifications is not null)
        {
            foreach (TripScheduledNotificationData notification in cascadeNotifications)
                await _notificationDispatcher.DispatchAsync(notification);
        }
    }
}
