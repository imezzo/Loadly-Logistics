using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverDocuments;

public class DownloadDriverDocumentAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IMediaService _mediaService;

    public DownloadDriverDocumentAction(
        IDriverDocumentRepository driverDocumentRepository,
        IMediaService mediaService)
    {
        _driverDocumentRepository = driverDocumentRepository;
        _mediaService = mediaService;
    }

    public async Task<string> ExecuteAsync(DownloadDriverDocumentDto dto)
    {
        var driverDocument = await _driverDocumentRepository.GetByIdOrThrowAsync(dto.Id);

        if (string.IsNullOrWhiteSpace(driverDocument.Path))
        {
            throw new NotFoundException("driver_documents.no_file", dto.Id);
        }

        if (driverDocument.Document == null)
        {
            throw new NotFoundException("driver_documents.document_info_not_found", dto.Id);
        }

        var modelName = "driver";
        var documentNameEn = driverDocument.Document.NameEn.ToLower().Replace(" ", "-");
        var fullPath = $"{modelName}/{driverDocument.DriverId}/{documentNameEn}/{driverDocument.Path}";

        return _mediaService.Read(fullPath);
    }
}
