using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Taxes;

public class ListTaxesAction
{
    private readonly ITaxRepository _taxRepository;
    private readonly IMapper _mapper;

    public ListTaxesAction(ITaxRepository taxRepository, IMapper mapper)
    {
        _taxRepository = taxRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<TaxDto>> ExecuteAsync(ListTaxesDto dto)
    {
        var (taxes, totalCount) = await _taxRepository.ListAsync(dto.Search, dto.Type, dto.Status, dto.Page, dto.PageSize);
        return new ListingDto<TaxDto>
        {
            Data = _mapper.Map<List<TaxDto>>(taxes),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
