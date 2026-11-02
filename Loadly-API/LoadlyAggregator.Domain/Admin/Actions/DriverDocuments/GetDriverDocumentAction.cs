using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverDocuments;

public class GetDriverDocumentAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetDriverDocumentAction(
        IDriverDocumentRepository driverDocumentRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _driverDocumentRepository = driverDocumentRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<DriverDocumentDto> ExecuteAsync(GetDriverDocumentDto dto)
    {
        var driverDocument = await _driverDocumentRepository.GetByIdOrThrowAsync(dto.Id);
        var driverDocumentDto = _mapper.Map<DriverDocumentDto>(driverDocument);

        if (!string.IsNullOrWhiteSpace(driverDocumentDto.Path) && driverDocumentDto.Document != null)
        {
            var modelName = "driver";
            var documentNameEn = driverDocumentDto.Document.NameEn.ToLower().Replace(" ", "-");
            var fullPath = $"{modelName}/{driverDocumentDto.DriverId}/{documentNameEn}/{driverDocumentDto.Path}";
            driverDocumentDto.FileUrl = _mediaService.Read(fullPath);
        }

        return driverDocumentDto;
    }
}

