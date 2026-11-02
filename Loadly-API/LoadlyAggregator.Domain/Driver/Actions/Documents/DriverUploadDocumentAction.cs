using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Documents;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Documents;

public class DriverUploadDocumentAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IMediaService _mediaService;

    public DriverUploadDocumentAction(
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IDriverRepository driverRepository,
        IMediaService mediaService)
    {
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _driverRepository = driverRepository;
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(DriverUploadDocumentDto dto)
    {
        var document = await _documentRepository.GetByIdOrThrowAsync(dto.DocumentId);
        if (document.EntityId != DocumentEntityType.DRIVER)
        {
            throw new ValidationException("documents.invalid_document_type_for_driver");
        }

        _ = await _driverRepository.GetByIdOrThrowAsync(dto.DriverId);

        DocumentFileTypeValidator.Validate(dto.File.ContentType);

        var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
        var directoryPath = $"driver/{dto.DriverId}/{documentNameEn}";
        var filePath = await _mediaService.UploadAsync(dto.File, directoryPath);

        await _driverDocumentRepository.AddAsync(new DriverDocument
        {
            DocumentId = dto.DocumentId,
            DriverId = dto.DriverId,
            Path = filePath,
            ExpiryDate = dto.ExpiryDate,
            Status = DocumentVerificationStatus.PENDING,
            AddedBy = dto.DriverId
        });
        await _driverDocumentRepository.SaveChangesAsync();
    }
}
