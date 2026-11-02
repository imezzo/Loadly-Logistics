using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverLicenseTypes;

public class ListDriverLicenseTypesAction
{
    private readonly IDriverLicenseTypeRepository _driverLicenseTypeRepository;
    private readonly IMapper _mapper;

    public ListDriverLicenseTypesAction(
        IDriverLicenseTypeRepository driverLicenseTypeRepository,
        IMapper mapper)
    {
        _driverLicenseTypeRepository = driverLicenseTypeRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DriverLicenseTypeDto>> ExecuteAsync(ListDriverLicenseTypesDto dto)
    {
        var (driverLicenseTypes, totalCount) = await _driverLicenseTypeRepository.ListAsync(
            dto.Search,
            dto.Status,
            dto.Page,
            dto.PageSize);
        return new ListingDto<DriverLicenseTypeDto>
        {
            Data = _mapper.Map<List<DriverLicenseTypeDto>>(driverLicenseTypes),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
