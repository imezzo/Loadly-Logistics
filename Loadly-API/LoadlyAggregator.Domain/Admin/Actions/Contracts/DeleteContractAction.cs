using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class DeleteContractAction
{
    private readonly IContractRepository _contractRepository;

    public DeleteContractAction(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task ExecuteAsync(DeleteContractDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.Id);
        _contractRepository.Remove(contract);
        await _contractRepository.SaveChangesAsync();
    }
}
