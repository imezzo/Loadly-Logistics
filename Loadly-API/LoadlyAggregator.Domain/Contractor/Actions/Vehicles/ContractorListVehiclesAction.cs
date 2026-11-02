using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Vehicles;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Contractor.Actions.Vehicles;

public class ContractorListVehiclesAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public ContractorListVehiclesAction(
        IVehicleRepository vehicleRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<ListingDto<VehicleDto>> ExecuteAsync(ContractorListVehiclesDto dto)
    {
        var contractorIds = new List<long> { dto.ContractorId };

        var (vehicles, totalCount) = await _vehicleRepository.ListWithChildrenAsync(
            search: dto.Search,
            contractId: null,
            isAssignedToContract: null,
            colorId: null,
            capacityUnitId: null,
            vehicleTypeId: dto.VehicleTypeId,
            vehicleTypeIds: dto.VehicleTypeIds,
            headLookupForTailTypeIds: null,
            vehicleUnitType: dto.VehicleUnitType,
            vehicleTypeCategories: dto.VehicleTypeCategories,
            status: dto.Status,
            isAvailableTail: dto.IsAvailableTail,
            driverId: dto.DriverId,
            contractorIds: contractorIds,
            hasContractor: true,
            hasDriver: dto.HasDriver,
            orderByCompletedTripsDescending: null,
            page: dto.Page,
            pageSize: dto.PageSize);

        if (vehicles.Count == 0)
        {
            return new ListingDto<VehicleDto>
            {
                Data = new List<VehicleDto>(),
                Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
            };
        }

        var data = vehicles
            .Select(vehicle =>
            {
                var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                vehicleDto.CompletedTripsCount = vehicle.Trips.Count(t =>
                    t.Status == TripStatus.Completed);

                foreach (var doc in vehicleDto.VehicleDocuments.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
                {
                    var documentNameEn = doc.Document!.NameEn.ToLower().Replace(" ", "-");
                    doc.FileUrl = _mediaService.Read($"vehicle/{vehicleDto.Id}/{documentNameEn}/{doc.Path}");
                }

                return vehicleDto;
            })
            .ToList();

        return new ListingDto<VehicleDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
