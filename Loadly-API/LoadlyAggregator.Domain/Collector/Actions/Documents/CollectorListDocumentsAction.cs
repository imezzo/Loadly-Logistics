using AutoMapper;
using LoadlyAggregator.Domain.Collector.DTOs.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Collector.Actions.Documents;

public class CollectorListDocumentsAction
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public CollectorListDocumentsAction(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DocumentDto>> ExecuteAsync(CollectorListDocumentsDto dto)
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
