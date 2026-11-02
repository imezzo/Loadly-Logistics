using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.TripsManagement;

public class ExitsCustomsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IMediaService _mediaService;
    private readonly ICurrentUserService _currentUserService;

    public ExitsCustomsAction(
        ITripRepository tripRepository,
        IRecordingRepository recordingRepository,
        IMediaService mediaService,
        ICurrentUserService currentUserService)
    {
        _tripRepository = tripRepository;
        _recordingRepository = recordingRepository;
        _mediaService = mediaService;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(ExitsCustomsDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        _ = trip.DriverId ?? throw new ValidationException(TripMessageKeys.VehicleMustHaveAssignedDriver);

        if (trip.CustomsStatus == TripCustomsStatus.Cleared)
        {
            return;
        }

        TripValidator.ValidateDriverExitsCustomsEligibility(trip);

        List<string>? customsReceiptPaths = null;
        if (dto.CustomsReceipts != null && dto.CustomsReceipts.Any(i => i.Length > 0))
        {
            customsReceiptPaths = await _mediaService.UploadImagesAsync(
                dto.CustomsReceipts,
                TripStoragePathHelper.CustomsReceipts(dto.TripId));
        }

        var recording = new Recording
        {
            RecordableType = RecordableType.TRIP,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.TRIP_CUSTOMS_EXITED,
            AddedBy = _currentUserService.GetCurrentUserId(),
            AddedByModelType = "Admin",
            Data = JsonSerializer.SerializeToDocument(new DriverExitsCustomsEventData
            {
                CustomsTotalAmount = dto.CustomsTotalAmount,
                Note = dto.Note,
                CustomsReceiptPaths = customsReceiptPaths,
                Location = dto.CurrentLocation
            })
        };
        await _recordingRepository.AddAsync(recording);

        trip.CustomsStatus = TripCustomsStatus.Cleared;
        trip.CustomsTotalAmount = dto.CustomsTotalAmount;
        _tripRepository.Update(trip);

        await _tripRepository.SaveChangesAsync();
    }
}
