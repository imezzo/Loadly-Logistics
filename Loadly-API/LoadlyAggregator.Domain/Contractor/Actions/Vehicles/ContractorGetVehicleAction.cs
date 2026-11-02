using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Vehicles;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Vehicles;

public class ContractorGetVehicleAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public ContractorGetVehicleAction(
        IVehicleRepository vehicleRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<VehicleDto> ExecuteAsync(ContractorGetVehicleDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdOrThrowAsync(dto.Id);

        if (vehicle.ContractorId != dto.ContractorId)
        {
            throw new ForbiddenException("vehicles.not_authorized");
        }

        var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
        vehicleDto.CompletedTripsCount = vehicle.Trips.Count(t =>
            t.Status == TripStatus.Completed);

        foreach (var doc in vehicleDto.VehicleDocuments.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
        {
            var documentNameEn = doc.Document!.NameEn.ToLower().Replace(" ", "-");
            doc.FileUrl = _mediaService.Read($"vehicle/{vehicleDto.Id}/{documentNameEn}/{doc.Path}");
        }

        return vehicleDto;
    }
}
