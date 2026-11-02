using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class UpdateTripDispatcherNotesAction
{
    private readonly ITripRepository _tripRepository;

    public UpdateTripDispatcherNotesAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task ExecuteAsync(UpdateTripDispatcherNotesDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.Id);

        trip.DispatcherNotes = string.IsNullOrWhiteSpace(dto.DispatcherNotes) ? null : dto.DispatcherNotes.Trim();
        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();
    }
}
