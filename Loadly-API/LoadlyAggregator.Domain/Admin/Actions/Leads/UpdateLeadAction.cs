using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Leads;

public class UpdateLeadAction
{
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadAction(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task ExecuteAsync(UpdateLeadDto dto)
    {
        var lead = await _leadRepository.GetByIdOrThrowAsync(dto.Id);
        lead.FullName = dto.FullName;
        lead.Phone = dto.Phone;
        lead.Email = dto.Email;
        await _leadRepository.UpdateAsync(lead);
        await _leadRepository.SaveChangesAsync();
    }
}
