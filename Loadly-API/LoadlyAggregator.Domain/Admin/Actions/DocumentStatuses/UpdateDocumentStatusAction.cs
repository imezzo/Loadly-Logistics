using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses;

public class UpdateDocumentStatusAction
{
    private readonly IDocumentStatusRepository _documentStatusRepository;

    public UpdateDocumentStatusAction(IDocumentStatusRepository documentStatusRepository)
    {
        _documentStatusRepository = documentStatusRepository;
    }

    public async Task ExecuteAsync(UpdateDocumentStatusDto dto)
    {
        var documentStatus = await _documentStatusRepository.GetByIdOrThrowAsync(dto.Id);
        documentStatus.NameEn = dto.NameEn;
        documentStatus.NameAr = dto.NameAr;
        await _documentStatusRepository.UpdateAsync(documentStatus);
        await _documentStatusRepository.SaveChangesAsync();
    }
}
