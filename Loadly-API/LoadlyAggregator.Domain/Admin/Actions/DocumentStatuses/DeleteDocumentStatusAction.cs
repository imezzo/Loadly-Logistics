using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses;

public class DeleteDocumentStatusAction
{
    private readonly IDocumentStatusRepository _documentStatusRepository;

    public DeleteDocumentStatusAction(IDocumentStatusRepository documentStatusRepository)
    {
        _documentStatusRepository = documentStatusRepository;
    }

    public async Task ExecuteAsync(DeleteDocumentStatusDto dto)
    {
        var documentStatus = await _documentStatusRepository.GetByIdOrThrowAsync(dto.Id);
        _documentStatusRepository.Remove(documentStatus);
        await _documentStatusRepository.SaveChangesAsync();
    }
}
