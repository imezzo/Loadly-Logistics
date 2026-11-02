using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverDocuments;

public class UpdateDriverDocumentAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDriverRepository _driverRepository;

    public UpdateDriverDocumentAction(
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IDriverRepository driverRepository)
    {
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _driverRepository = driverRepository;
    }

    public async Task ExecuteAsync(UpdateDriverDocumentDto dto)
    {
        var driverDocument = await _driverDocumentRepository.GetByIdOrThrowAsync(dto.Id);
        _ = await _documentRepository.GetByIdOrThrowAsync(dto.DocumentId);
        _ = await _driverRepository.GetByIdOrThrowAsync(dto.DriverId);

        driverDocument.DocumentId = dto.DocumentId;
        driverDocument.DriverId = dto.DriverId;
        driverDocument.ExpiryDate = dto.ExpiryDate;
        driverDocument.Status = dto.Status;
        await _driverDocumentRepository.UpdateAsync(driverDocument);
        await _driverDocumentRepository.SaveChangesAsync();
    }
}
