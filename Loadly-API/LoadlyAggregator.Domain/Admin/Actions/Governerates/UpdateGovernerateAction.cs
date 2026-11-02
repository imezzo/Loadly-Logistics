using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Governerates;

public class UpdateGovernerateAction
{
    private readonly IGovernerateRepository _governerateRepository;

    public UpdateGovernerateAction(IGovernerateRepository governerateRepository)
    {
        _governerateRepository = governerateRepository;
    }

    public async Task ExecuteAsync(UpdateGovernerateDto dto)
    {
        var governerate = await _governerateRepository.GetByIdOrThrowAsync(dto.Id);
        governerate.NameEn = dto.NameEn;
        governerate.NameAr = dto.NameAr;
        await _governerateRepository.UpdateAsync(governerate);
        await _governerateRepository.SaveChangesAsync();
    }
}
