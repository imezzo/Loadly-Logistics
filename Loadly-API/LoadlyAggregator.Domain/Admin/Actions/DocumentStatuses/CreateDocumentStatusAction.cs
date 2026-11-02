using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses;

public class CreateDocumentStatusAction
{
    private readonly IDocumentStatusRepository _documentStatusRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateDocumentStatusAction(
        IDocumentStatusRepository documentStatusRepository,
        ICurrentUserService currentUserService)
    {
        _documentStatusRepository = documentStatusRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateDocumentStatusDto dto)
    {
        await _documentStatusRepository.AddAsync(new LookupDocumentStatus
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _documentStatusRepository.SaveChangesAsync();
    }
}
