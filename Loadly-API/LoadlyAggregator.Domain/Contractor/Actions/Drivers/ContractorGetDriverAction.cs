using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Drivers;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Drivers;

public class ContractorGetDriverAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public ContractorGetDriverAction(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<DriverDto> ExecuteAsync(ContractorGetDriverDto dto)
    {
        var driver = await _driverRepository.GetByIdOrThrowAsync(dto.Id);

        if (driver.ContractorId != dto.ContractorId)
        {
            throw new ForbiddenException("drivers.not_authorized");
        }

        var driverDto = _mapper.Map<DriverDto>(driver);
        driverDto.CompletedTripsCount = driver.Trips.Count(t =>
            t.Status == TripStatus.Completed);

        return driverDto;
    }
}
