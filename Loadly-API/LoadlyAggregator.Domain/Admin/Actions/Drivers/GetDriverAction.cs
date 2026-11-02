using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.DTOs.Addresses;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class GetDriverAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverDetailRepository _driverDetailRepository;
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetDriverAction(
        IDriverRepository driverRepository,
        IDriverDetailRepository driverDetailRepository,
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _driverRepository = driverRepository;
        _driverDetailRepository = driverDetailRepository;
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<DriverDto> ExecuteAsync(GetDriverDto dto)
    {
        var driver = await _driverRepository.GetByIdOrThrowAsync(dto.Id);

        var driverDto = _mapper.Map<DriverDto>(driver);
        driverDto.CompletedTripsCount = driver.Trips.Count(t => t.Status == TripStatus.Completed);

        var driverDetail = await _driverDetailRepository.GetByDriverIdAsync(driver.Id);
        if (driverDetail != null)
        {
            driverDto.DriverDetail = _mapper.Map<DriverDetailDto>(driverDetail);
        }

        var (documentsList, _) = await _driverDocumentRepository.ListAsync(driverId: driver.Id);
        var documents = documentsList.ToList();
        driverDto.Documents = _mapper.Map<List<DriverDocumentDto>>(documents);

        foreach (var doc in documents)
        {
            var document = await _documentRepository.GetByIdAsync(doc.DocumentId);
            if (document != null && !string.IsNullOrWhiteSpace(doc.Path))
            {
                var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
                var fileUrl = _mediaService.Read($"driver/{driverDto.Id}/{documentNameEn}/{doc.Path}");
                var docDto = driverDto.Documents.FirstOrDefault(d => d.Id == doc.Id);
                if (docDto != null)
                {
                    docDto.FileUrl = fileUrl;
                }
            }
        }

        var vehicle = driver.Vehicles.FirstOrDefault(v => v.HeadVehicleId == null) ?? driver.Vehicles.FirstOrDefault();
        if (vehicle != null)
            driverDto.Vehicle = _mapper.Map<VehicleRelationDto>(vehicle);

        return driverDto;
    }
}
