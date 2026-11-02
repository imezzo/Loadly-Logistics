using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class CancelTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly IAdminRepository _adminRepository;
    private readonly INotificationEventDispatcher _notificationDispatcher;
    private readonly ITripBookingRequestRepository _tripBookingRequestRepository;
    private readonly ITripBookingInvitationRepository _tripBookingInvitationRepository;
    private readonly ITripBookingInvitationRoundRepository _invitationRoundRepository;

    public CancelTripAction(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        IRecordingRepository recordingRepository,
        ICurrentUserService currentUserService,
        IPermissionService permissionService,
        IAdminRepository adminRepository,
        INotificationEventDispatcher notificationDispatcher,
        ITripBookingRequestRepository tripBookingRequestRepository,
        ITripBookingInvitationRepository tripBookingInvitationRepository,
        ITripBookingInvitationRoundRepository invitationRoundRepository)
    {
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _driverRepository = driverRepository;
        _recordingRepository = recordingRepository;
        _currentUserService = currentUserService;
        _permissionService = permissionService;
        _adminRepository = adminRepository;
        _notificationDispatcher = notificationDispatcher;
        _tripBookingRequestRepository = tripBookingRequestRepository;
        _tripBookingInvitationRepository = tripBookingInvitationRepository;
        _invitationRoundRepository = invitationRoundRepository;
    }

    public async Task ExecuteAsync(CancelTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.Id);

        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");
        var admin = await _adminRepository.GetByIdOrThrowAsync(currentUserId);
        await TripValidator.ValidateCancelTripPermissionByStatusAsync(admin, trip, _permissionService);
        if (trip.Status > TripStatus.InProgress)
        {
            throw new ValidationException(TripMessageKeys.CanOnlyCancelBeforeOrInProgress);
        }

        // Capture before mutation. The driver only needs to know when the trip was visible to
        // them (Scheduled or InProgress) — for Requested/Pending no driver has been assigned yet.
        long? assignedDriverId = trip.Status is TripStatus.Scheduled or TripStatus.InProgress
            ? trip.DriverId
            : null;
        long? companyId = trip.Contract?.CompanyId;
        long? contractorId = trip.ContractorId;

        if (trip.Status == TripStatus.InProgress)
        {
            if (trip.VehicleId.HasValue)
            {
                await _vehicleRepository.UpdateStatusAsync(trip.VehicleId.Value, VehicleStatus.Available);
                if (trip.TailVehicleId.HasValue)
                {
                    await _vehicleRepository.UpdateStatusAsync(trip.TailVehicleId.Value, VehicleStatus.Available);
                }
            }

            if (trip.DriverId.HasValue)
            {
                await _driverRepository.UpdateWorkingStatusAsync(trip.DriverId.Value, DriverWorkingStatus.Available);
            }
        }

        trip.Status = TripStatus.Cancelled;
        _tripRepository.Update(trip);

        await TripBookingHelper.CloseAsync(
            trip.Id,
            _tripBookingRequestRepository,
            _tripBookingInvitationRepository,
            _invitationRoundRepository,
            currentUserId);

        var cancelTripRecording = new Recording
        {
            RecordableType = RecordableType.TRIP,
            RecordableId = dto.Id,
            RecordableEventType = RecordableEventType.TRIP_CANCELLED,
            AddedBy = currentUserId,
            AddedByModelType = "Admin",
            Data = JsonSerializer.SerializeToDocument(new DriverCancelTripEventData
            {
                CancellationReason = dto.CancellationReason
            })
        };
        await _recordingRepository.AddAsync(cancelTripRecording);

        await _tripRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripCancelledNotificationData
        {
            TripId = trip.Id,
            CompanyId = companyId,
            DriverId = assignedDriverId,
            ContractorId = contractorId
        });
    }
}
