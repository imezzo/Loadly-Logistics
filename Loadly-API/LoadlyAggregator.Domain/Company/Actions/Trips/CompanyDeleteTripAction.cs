using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Trips;

public class CompanyDeleteTripAction
{
    private readonly ITripRepository _tripRepository;

    public CompanyDeleteTripAction(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task ExecuteAsync(CompanyDeleteTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        if (trip.Contract?.CompanyId != dto.CompanyId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        if (trip.Status != TripStatus.CompanyDraft)
            throw new ValidationException(TripMessageKeys.CanOnlyDeleteCompanyDraft);

        _tripRepository.Remove(trip);
        await _tripRepository.SaveChangesAsync();
    }
}
