using AutoMapper;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Documents;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Contractor.Actions.Documents;

public class ContractorListDocumentsAction
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public ContractorListDocumentsAction(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DocumentDto>> ExecuteAsync(ContractorListDocumentsDto dto)
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
