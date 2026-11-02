using AutoMapper;
using LoadlyAggregator.Domain.Collector.DTOs.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Collector.Actions.Documents;

public class CollectorGetDocumentAction
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public CollectorGetDocumentAction(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentDto> ExecuteAsync(CollectorGetDocumentDto dto)
    {
        var document = await _documentRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<DocumentDto>(document);
    }
}
