using AutoMapper;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorGetTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripBookingInvitationRepository _tripBookingInvitationRepository;
    private readonly IMapper _mapper;

    public ContractorGetTripAction(
        ITripRepository tripRepository,
        ITripBookingInvitationRepository tripBookingInvitationRepository,
        IMapper mapper)
    {
        _tripRepository = tripRepository;
        _tripBookingInvitationRepository = tripBookingInvitationRepository;
        _mapper = mapper;
    }

    public async Task<ContractorTripDto> ExecuteAsync(ContractorGetTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);

        var isAssigned = trip.ContractorId == dto.ContractorId;
        var invitation = isAssigned
            ? null
            : await _tripBookingInvitationRepository.GetByTripAndContractorAsync(dto.Id, dto.ContractorId);
        var isInvited = invitation is { Status: TripBookingInvitationStatus.Pending };

        if (!isAssigned && !isInvited)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        return _mapper.Map<ContractorTripDto>(trip);
    }
}
