using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses;

public class GetDocumentStatusAction
{
    private readonly IDocumentStatusRepository _documentStatusRepository;
    private readonly IMapper _mapper;

    public GetDocumentStatusAction(IDocumentStatusRepository documentStatusRepository, IMapper mapper)
    {
        _documentStatusRepository = documentStatusRepository;
        _mapper = mapper;
    }

    public async Task<DocumentStatusDto> ExecuteAsync(GetDocumentStatusDto dto)
    {
        var documentStatus = await _documentStatusRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<DocumentStatusDto>(documentStatus);
    }
}
