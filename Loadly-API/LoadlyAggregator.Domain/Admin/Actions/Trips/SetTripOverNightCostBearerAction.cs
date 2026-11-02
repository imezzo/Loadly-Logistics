using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class SetTripOverNightCostBearerAction
{
    private readonly ITripRepository _tripRepository;

    public SetTripOverNightCostBearerAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task ExecuteAsync(SetTripOverNightCostBearerDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.Id);

        if (trip.Status is TripStatus.Completed)
            throw new ValidationException("trips.overnight_bearer_not_updatable");

        trip.OverNightCostBearer = dto.OverNightCostBearer;
        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();
    }
}
