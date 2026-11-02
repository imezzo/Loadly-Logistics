using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Sectors;

public class GetSectorAction
{
    private readonly ISectorRepository _sectorRepository;
    private readonly IMapper _mapper;

    public GetSectorAction(ISectorRepository sectorRepository, IMapper mapper)
    {
        _sectorRepository = sectorRepository;
        _mapper = mapper;
    }

    public async Task<SectorDto> ExecuteAsync(GetSectorDto dto)
    {
        var sector = await _sectorRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<SectorDto>(sector);
    }
}
