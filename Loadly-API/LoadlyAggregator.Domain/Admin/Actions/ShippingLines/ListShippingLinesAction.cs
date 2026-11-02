using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.ShippingLines;

public class ListShippingLinesAction
{
    private readonly IShippingLineRepository _shippingLineRepository;
    private readonly IMapper _mapper;

    public ListShippingLinesAction(IShippingLineRepository shippingLineRepository, IMapper mapper)
    {
        _shippingLineRepository = shippingLineRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ShippingLineDto>> ExecuteAsync(ListShippingLinesDto dto)
    {
        (List<Common.Entities.ShippingLine> shippingLines, int totalCount) = await _shippingLineRepository.ListAsync(
            dto.Search,
            dto.Status,
            dto.Page,
            dto.PageSize);
        return new ListingDto<ShippingLineDto>
        {
            Data = _mapper.Map<List<ShippingLineDto>>(shippingLines),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
