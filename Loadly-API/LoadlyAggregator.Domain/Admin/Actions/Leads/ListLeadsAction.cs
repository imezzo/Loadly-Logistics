using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Leads;

public class ListLeadsAction
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public ListLeadsAction(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<LeadDto>> ExecuteAsync(ListLeadsDto dto)
    {
        var (leads, totalCount) = await _leadRepository.ListAsync(dto.Search, dto.Page, dto.PageSize);

        if (leads.Count == 0)
        {
            return new ListingDto<LeadDto>
            {
                Data = new List<LeadDto>(),
                Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
            };
        }

        var countsById = await _leadRepository.GetAggregateCountsByIdsAsync(leads.Select(l => l.Id).ToList());

        var data = leads.Select(lead =>
        {
            var leadDto = _mapper.Map<LeadDto>(lead);
            if (countsById.TryGetValue(lead.Id, out var counts))
            {
                leadDto.TotalCompletedTrips = counts.TotalCompletedTrips;
            }

            return leadDto;
        }).ToList();

        return new ListingDto<LeadDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
