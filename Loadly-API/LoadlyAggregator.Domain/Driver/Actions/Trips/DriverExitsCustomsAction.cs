using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class DriverExitsCustomsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IMediaService _mediaService;

    public DriverExitsCustomsAction(
        ITripRepository tripRepository,
        IRecordingRepository recordingRepository,
        IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _recordingRepository = recordingRepository;
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(DriverExitsCustomsDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        if (trip.CustomsStatus == TripCustomsStatus.Cleared)
            return;

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
            AddedBy = dto.DriverId,
            AddedByModelType = "Driver",
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
