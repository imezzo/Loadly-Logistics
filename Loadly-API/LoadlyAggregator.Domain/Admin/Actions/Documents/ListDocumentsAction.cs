using AutoMapper;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Documents;

public class ListDocumentsAction
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public ListDocumentsAction(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DocumentDto>> ExecuteAsync(ListDocumentsDto dto)
    {
        var (documents, totalCount) = await _documentRepository.ListAsync(
            dto.Search,
            dto.EntityId,
            dto.Required,
            dto.Status,
            dto.Page,
            dto.PageSize);
        return new ListingDto<DocumentDto>
        {
            Data = _mapper.Map<List<DocumentDto>>(documents),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
