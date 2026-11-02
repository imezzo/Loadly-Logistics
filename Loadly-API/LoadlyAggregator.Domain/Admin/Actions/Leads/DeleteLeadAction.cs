using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Leads;

public class DeleteLeadAction
{
    private readonly ILeadRepository _leadRepository;

    public DeleteLeadAction(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task ExecuteAsync(DeleteLeadDto dto)
    {
        var lead = await _leadRepository.GetByIdOrThrowAsync(dto.Id);
        _leadRepository.Remove(lead);
        await _leadRepository.SaveChangesAsync();
    }
}
