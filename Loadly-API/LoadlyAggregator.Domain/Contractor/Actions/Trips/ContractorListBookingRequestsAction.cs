using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorListBookingRequestsAction
{
    private readonly ITripBookingRequestRepository _bookingRequestRepository;
    private readonly IMapper _mapper;

    public ContractorListBookingRequestsAction(
        ITripBookingRequestRepository bookingRequestRepository,
        IMapper mapper)
    {
        _bookingRequestRepository = bookingRequestRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ContractorTripBookingRequestDto>> ExecuteAsync(ContractorListBookingRequestsDto dto)
    {
        var (items, totalCount) = await _bookingRequestRepository.ListByContractorIdAsync(
            dto.ContractorId,
            dto.Status,
            dto.TripId,
            dto.StartDateFrom,
            dto.StartDateTo,
            dto.VehicleTypeId,
            dto.Search,
            dto.Page,
            dto.PageSize);

        return new ListingDto<ContractorTripBookingRequestDto>
        {
            Data = _mapper.Map<List<ContractorTripBookingRequestDto>>(items),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
