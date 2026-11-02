using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Taxes;

public class GetTaxAction
{
    private readonly ITaxRepository _taxRepository;
    private readonly IMapper _mapper;

    public GetTaxAction(ITaxRepository taxRepository, IMapper mapper)
    {
        _taxRepository = taxRepository;
        _mapper = mapper;
    }

    public async Task<TaxDto> ExecuteAsync(GetTaxDto dto)
    {
        var tax = await _taxRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<TaxDto>(tax);
    }
}
