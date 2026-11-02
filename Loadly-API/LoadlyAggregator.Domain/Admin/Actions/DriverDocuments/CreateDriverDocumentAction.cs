using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverDocuments;

public class CreateDriverDocumentAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IMediaService _mediaService;
    private readonly ICurrentUserService _currentUserService;

    public CreateDriverDocumentAction(
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IDriverRepository driverRepository,
        IMediaService mediaService,
        ICurrentUserService currentUserService)
    {
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _driverRepository = driverRepository;
        _mediaService = mediaService;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateDriverDocumentDto dto)
    {
        var document = await _documentRepository.GetByIdOrThrowAsync(dto.DocumentId);
        _ = await _driverRepository.GetByIdOrThrowAsync(dto.DriverId);

        string? filePath = null;
        if (dto.File != null && dto.File.Length > 0)
        {
            DocumentFileTypeValidator.Validate(dto.File.ContentType);

            var modelName = "driver";
            var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
            var directoryPath = $"{modelName}/{dto.DriverId}/{documentNameEn}";
            filePath = await _mediaService.UploadAsync(dto.File, directoryPath);
        }

        await _driverDocumentRepository.AddAsync(new DriverDocument
        {
            DocumentId = dto.DocumentId,
            DriverId = dto.DriverId,
            Path = filePath,
            ExpiryDate = dto.ExpiryDate,
            Status = dto.Status,
            AddedBy = dto.AddedBy ?? _currentUserService.GetCurrentUserId()
        });
        await _driverDocumentRepository.SaveChangesAsync();
    }
}
