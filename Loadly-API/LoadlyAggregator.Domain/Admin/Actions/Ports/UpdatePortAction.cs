using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Ports;

public class UpdatePortAction
{
    private readonly IPortRepository _portRepository;

    public UpdatePortAction(IPortRepository portRepository)
    {
        _portRepository = portRepository;
    }

    public async Task ExecuteAsync(UpdatePortDto dto)
    {
        Common.Entities.Port port = await _portRepository.GetByIdOrThrowAsync(dto.Id);
        port.NameAr = dto.NameAr;
        port.NameEn = dto.NameEn;
        port.Country = dto.Country;
        port.PortCode = dto.PortCode;
        port.Type = dto.Type;
        port.Status = dto.Status;
        await _portRepository.UpdateAsync(port);
        await _portRepository.SaveChangesAsync();
    }
}
