using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class GetVehicleAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetVehicleAction(
        IVehicleRepository vehicleRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<VehicleDto> ExecuteAsync(GetVehicleDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdOrThrowAsync(dto.Id);
        var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
        vehicleDto.CompletedTripsCount = vehicle.Trips.Count(t => t.Status == TripStatus.Completed);
        foreach (var doc in vehicleDto.VehicleDocuments.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
        {
            var documentNameEn = doc.Document!.NameEn.ToLower().Replace(" ", "-");
            doc.FileUrl = _mediaService.Read($"vehicle/{vehicleDto.Id}/{documentNameEn}/{doc.Path}");
        }
        return vehicleDto;
    }
}
