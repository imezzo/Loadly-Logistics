using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods;

public class ListCommunicationMethodsAction
{
    private readonly ICommunicationMethodRepository _communicationMethodRepository;
    private readonly IMapper _mapper;

    public ListCommunicationMethodsAction(
        ICommunicationMethodRepository communicationMethodRepository,
        IMapper mapper)
    {
        _communicationMethodRepository = communicationMethodRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<CommunicationMethodDto>> ExecuteAsync(ListCommunicationMethodsDto dto)
    {
        var (communicationMethods, totalCount) = await _communicationMethodRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<CommunicationMethodDto>
        {
            Data = _mapper.Map<List<CommunicationMethodDto>>(communicationMethods),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
