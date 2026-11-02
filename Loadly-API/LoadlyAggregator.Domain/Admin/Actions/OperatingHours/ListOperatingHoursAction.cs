using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.OperatingHours;

public class ListOperatingHoursAction
{
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly IMapper _mapper;

    public ListOperatingHoursAction(
        IOperatingHoursRepository operatingHoursRepository,
        IMapper mapper)
    {
        _operatingHoursRepository = operatingHoursRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<OperatingHoursDto>> ExecuteAsync(ListOperatingHoursDto dto)
    {
        var (operatingHours, totalCount) = await _operatingHoursRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<OperatingHoursDto>
        {
            Data = _mapper.Map<List<OperatingHoursDto>>(operatingHours),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
