using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts;

public class ListDiscountsAction
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public ListDiscountsAction(
        IDiscountRepository discountRepository,
        IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DiscountDto>> ExecuteAsync(ListDiscountsDto dto)
    {
        var (discounts, totalCount) = await _discountRepository.ListAsync(
            dto.Search,
            dto.Type,
            dto.Status,
            dto.Page,
            dto.PageSize);
        return new ListingDto<DiscountDto>
        {
            Data = _mapper.Map<List<DiscountDto>>(discounts),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
