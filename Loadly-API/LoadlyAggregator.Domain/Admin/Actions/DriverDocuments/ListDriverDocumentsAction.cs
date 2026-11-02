using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverDocuments;

public class ListDriverDocumentsAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public ListDriverDocumentsAction(
        IDriverDocumentRepository driverDocumentRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _driverDocumentRepository = driverDocumentRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<ListingDto<DriverDocumentDto>> ExecuteAsync(ListDriverDocumentsDto dto)
    {
        var (driverDocuments, totalCount) = await _driverDocumentRepository.ListAsync(
            driverId: dto.DriverId,
            documentId: dto.DocumentId,
            status: dto.Status,
            page: dto.Page,
            pageSize: dto.PageSize);

        var driverDocumentDtos = _mapper.Map<List<DriverDocumentDto>>(driverDocuments);

        foreach (var driverDocumentDto in driverDocumentDtos)
        {
            if (!string.IsNullOrWhiteSpace(driverDocumentDto.Path) && driverDocumentDto.Document != null)
            {
                var modelName = "driver";
                var documentNameEn = driverDocumentDto.Document.NameEn.ToLower().Replace(" ", "-");
                var fullPath = $"{modelName}/{driverDocumentDto.DriverId}/{documentNameEn}/{driverDocumentDto.Path}";
                driverDocumentDto.FileUrl = _mediaService.Read(fullPath);
            }
        }

        return new ListingDto<DriverDocumentDto>
        {
            Data = driverDocumentDtos,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}

