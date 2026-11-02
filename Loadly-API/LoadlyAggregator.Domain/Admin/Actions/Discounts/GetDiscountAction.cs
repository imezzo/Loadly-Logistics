using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts;

public class GetDiscountAction
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public GetDiscountAction(
        IDiscountRepository discountRepository,
        IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<DiscountDto> ExecuteAsync(GetDiscountDto dto)
    {
        var discount = await _discountRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<DiscountDto>(discount);
    }
}
