using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverLicenseTypes;

public class GetDriverLicenseTypeAction
{
    private readonly IDriverLicenseTypeRepository _driverLicenseTypeRepository;
    private readonly IMapper _mapper;

    public GetDriverLicenseTypeAction(
        IDriverLicenseTypeRepository driverLicenseTypeRepository,
        IMapper mapper)
    {
        _driverLicenseTypeRepository = driverLicenseTypeRepository;
        _mapper = mapper;
    }

    public async Task<DriverLicenseTypeDto> ExecuteAsync(GetDriverLicenseTypeDto dto)
    {
        var driverLicenseType = await _driverLicenseTypeRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<DriverLicenseTypeDto>(driverLicenseType);
    }
}
