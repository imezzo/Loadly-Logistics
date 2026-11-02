using AutoMapper;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Documents;

public class GetDocumentAction
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public GetDocumentAction(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentDto> ExecuteAsync(GetDocumentDto dto)
    {
        var document = await _documentRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<DocumentDto>(document);
    }
}
