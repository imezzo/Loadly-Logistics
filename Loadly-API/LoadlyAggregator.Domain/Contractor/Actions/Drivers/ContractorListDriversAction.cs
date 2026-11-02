using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Drivers;
using LoadlyAggregator.Shared.DTOs.Common;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Drivers;

public class ContractorListDriversAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public ContractorListDriversAction(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DriverDto>> ExecuteAsync(ContractorListDriversDto dto)
    {
        var contractorIds = new List<long> { dto.ContractorId };

        var (drivers, totalCount) = await _driverRepository.ListAsync(
            dto.Search,
            dto.Status,
            contractorIds,
            hasContractor: true,
            dto.IsAvailable,
            dto.LicenseCategory,
            dto.WorkingStatuses,
            dto.Page,
            dto.PageSize);

        var driverDtos = drivers.Select(driver =>
        {
            var driverDto = _mapper.Map<DriverDto>(driver);
            driverDto.CompletedTripsCount = driver.Trips.Count(t =>
                t.Status == TripStatus.Completed);
            return driverDto;
        }).ToList();

        return new ListingDto<DriverDto>
        {
            Data = driverDtos,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
