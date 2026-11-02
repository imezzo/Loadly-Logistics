using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Documents;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Documents;

public class DriverUpdateDocumentAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IMediaService _mediaService;

    public DriverUpdateDocumentAction(
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IMediaService mediaService)
    {
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(DriverUpdateDocumentDto dto)
    {
        var driverDocument = await _driverDocumentRepository.GetByIdOrThrowAsync(dto.Id);

        if (driverDocument.DriverId != dto.DriverId)
        {
            throw new ForbiddenException("driver_documents.not_authorized_to_update");
        }

        if (dto.File != null && dto.File.Length > 0)
        {
            DocumentFileTypeValidator.Validate(dto.File.ContentType);

            var document = await _documentRepository.GetByIdOrThrowAsync(driverDocument.DocumentId);
            var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
            var directoryPath = $"driver/{dto.DriverId}/{documentNameEn}";
            driverDocument.Path = await _mediaService.UploadAsync(dto.File, directoryPath);
        }

        if (dto.ExpiryDate.HasValue)
        {
            driverDocument.ExpiryDate = dto.ExpiryDate;
        }
        await _driverDocumentRepository.UpdateAsync(driverDocument);
        await _driverDocumentRepository.SaveChangesAsync();
    }
}
