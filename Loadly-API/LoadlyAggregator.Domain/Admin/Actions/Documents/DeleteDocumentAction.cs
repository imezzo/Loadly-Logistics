using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Documents;

public class DeleteDocumentAction
{
    private readonly IDocumentRepository _documentRepository;

    public DeleteDocumentAction(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task ExecuteAsync(DeleteDocumentDto dto)
    {
        var document = await _documentRepository.GetByIdOrThrowAsync(dto.Id);
        _documentRepository.Remove(document);
        await _documentRepository.SaveChangesAsync();
    }
}
