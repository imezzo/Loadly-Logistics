using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class GetTripRouteAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripRouteSyncRepository _tripRouteSyncRepository;

    public GetTripRouteAction(
        ITripRepository tripRepository,
        ITripRouteSyncRepository tripRouteSyncRepository)
    {
        _tripRepository = tripRepository;
        _tripRouteSyncRepository = tripRouteSyncRepository;
    }

    public async Task<TripRouteDto> ExecuteAsync(GetTripRouteDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        if (trip.Status != TripStatus.Completed)
        {
            throw new ValidationException(TripMessageKeys.TripMustBeCompleted);
        }

        var routeSync = await _tripRouteSyncRepository.GetByTripIdAsync(dto.TripId)
            ?? throw new NotFoundException(TripMessageKeys.RouteNotFound);

        return new TripRouteDto
        {
            Id = routeSync.Id,
            TripId = routeSync.TripId,
            DriverId = routeSync.DriverId,
            DistanceMeters = routeSync.DistanceMeters,
            StartTime = routeSync.StartTime,
            EndTime = routeSync.EndTime,
            GpsPacketCount = routeSync.GpsPacketCount,
            PointCount = routeSync.PointCount,
            CreatedAt = routeSync.CreatedAt,
            Route = routeSync.Payload.RootElement.Clone()
        };
    }
}
