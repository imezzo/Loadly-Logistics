using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ShippingLines;

public class GetShippingLineAction
{
    private readonly IShippingLineRepository _shippingLineRepository;
    private readonly IMapper _mapper;

    public GetShippingLineAction(IShippingLineRepository shippingLineRepository, IMapper mapper)
    {
        _shippingLineRepository = shippingLineRepository;
        _mapper = mapper;
    }

    public async Task<ShippingLineDto> ExecuteAsync(GetShippingLineDto dto)
    {
        Common.Entities.ShippingLine shippingLine = await _shippingLineRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<ShippingLineDto>(shippingLine);
    }
}
