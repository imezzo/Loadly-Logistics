using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class DriverSyncRouteAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripRouteSyncRepository _tripRouteSyncRepository;

    public DriverSyncRouteAction(
        ITripRepository tripRepository,
        ITripRouteSyncRepository tripRouteSyncRepository)
    {
        _tripRepository = tripRepository;
        _tripRouteSyncRepository = tripRouteSyncRepository;
    }

    public async Task ExecuteAsync(DriverSyncRouteDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        if (trip.Status != TripStatus.Completed)
        {
            throw new ValidationException(TripMessageKeys.TripMustBeCompleted);
        }

        TripRouteSyncPayloadValidator.Validate(dto.Route);

        var routeSync = new TripRouteSync
        {
            TripId = dto.TripId,
            DriverId = dto.DriverId,
            DistanceMeters = dto.DistanceMeters,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            GpsPacketCount = dto.GpsPacketCount,
            PointCount = dto.Route.PointCount,
            Payload = JsonSerializer.SerializeToDocument(dto.Route)
        };

        await _tripRouteSyncRepository.AddAsync(routeSync);
        await _tripRouteSyncRepository.SaveChangesAsync();
    }
}
