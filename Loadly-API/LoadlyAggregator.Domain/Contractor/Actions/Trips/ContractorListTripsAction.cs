using AutoMapper;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorListTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;

    public ContractorListTripsAction(ITripRepository tripRepository, IMapper mapper)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ContractorTripDto>> ExecuteAsync(ContractorListTripsDto dto)
    {
        var (trips, totalCount) = await _tripRepository.ListAsync(
            contractId: null,
            contractReferenceNumber: null,
            companyId: null,
            matchingCompanyIds: null,
            companyName: null,
            driverName: null,
            plate: null,
            vehicleId: null,
            vehicleTypeId: dto.VehicleTypeId,
            sectorIds: null,
            driverId: dto.DriverId,
            leadIds: null,
            isExternalTrip: null,
            contractorIds: null,
            hasContractor: dto.HasContractor,
            status: dto.Status.HasValue ? (int?)dto.Status.Value : null,
            tripDocumentStatuses: null,
            overNightCostBearer: null,
            dispatchingMode: null,
            statuses: dto.Statuses is { Count: > 0 }
                ? dto.Statuses.Select(s => (int)s).ToArray()
                : null,
            startDateFrom: dto.StartDateFrom,
            startDateTo: dto.StartDateTo,
            currentDriverStatus: null,
            lifecycleActor: null,
            search: dto.Search,
            page: dto.Page,
            pageSize: dto.PageSize,
            invitedContractorId: dto.ContractorId,
            openInviteOnly: dto.OpenInviteOnly,
            isContractorPaid: dto.IsContractorPaid);

        var tripDtos = trips.Select(trip => _mapper.Map<ContractorTripDto>(trip)).ToList();

        return new ListingDto<ContractorTripDto>
        {
            Data = tripDtos,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
