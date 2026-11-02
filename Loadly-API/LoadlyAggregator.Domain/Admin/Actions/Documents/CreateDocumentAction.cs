using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Documents;

public class CreateDocumentAction
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateDocumentAction(
        IDocumentRepository documentRepository,
        ICurrentUserService currentUserService)
    {
        _documentRepository = documentRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateDocumentDto dto)
    {
        await _documentRepository.AddAsync(new Document
        {
            EntityId = dto.EntityId,
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            Required = dto.Required,
            Status = dto.Status,
            AddedBy = dto.AddedBy ?? _currentUserService.GetCurrentUserId()
        });
        await _documentRepository.SaveChangesAsync();
    }
}
