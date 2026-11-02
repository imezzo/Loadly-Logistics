using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Documents;

public class UpdateDocumentAction
{
    private readonly IDocumentRepository _documentRepository;

    public UpdateDocumentAction(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task ExecuteAsync(UpdateDocumentDto dto)
    {
        var document = await _documentRepository.GetByIdOrThrowAsync(dto.Id);
        document.NameEn = dto.NameEn;
        document.NameAr = dto.NameAr;
        document.Required = dto.Required;
        document.Status = dto.Status;
        await _documentRepository.UpdateAsync(document);
        await _documentRepository.SaveChangesAsync();
    }
}
