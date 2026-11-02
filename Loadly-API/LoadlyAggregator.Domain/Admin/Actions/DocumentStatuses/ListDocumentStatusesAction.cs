using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses;

public class ListDocumentStatusesAction
{
    private readonly IDocumentStatusRepository _documentStatusRepository;
    private readonly IMapper _mapper;

    public ListDocumentStatusesAction(IDocumentStatusRepository documentStatusRepository, IMapper mapper)
    {
        _documentStatusRepository = documentStatusRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DocumentStatusDto>> ExecuteAsync(BasePagedRequest dto)
    {
        var (documentStatuses, totalCount) = await _documentStatusRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<DocumentStatusDto>
        {
            Data = _mapper.Map<List<DocumentStatusDto>>(documentStatuses),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
