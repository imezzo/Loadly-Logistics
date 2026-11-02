using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorGetBookingRequestAction
{
    private readonly ITripBookingRequestRepository _bookingRequestRepository;
    private readonly IMapper _mapper;

    public ContractorGetBookingRequestAction(
        ITripBookingRequestRepository bookingRequestRepository,
        IMapper mapper)
    {
        _bookingRequestRepository = bookingRequestRepository;
        _mapper = mapper;
    }

    public async Task<ContractorTripBookingRequestDto> ExecuteAsync(ContractorGetBookingRequestDto dto)
    {
        var request = await _bookingRequestRepository.GetByIdOrThrowAsync(dto.Id);
        if (request.ContractorId != dto.ContractorId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        return _mapper.Map<ContractorTripBookingRequestDto>(request);
    }
}
