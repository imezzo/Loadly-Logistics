using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Taxes;

public class DeleteTaxAction
{
    private readonly ITaxRepository _taxRepository;

    public DeleteTaxAction(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository;
    }

    public async Task ExecuteAsync(DeleteTaxDto dto)
    {
        var tax = await _taxRepository.GetByIdOrThrowAsync(dto.Id);
        _taxRepository.Remove(tax);
        await _taxRepository.SaveChangesAsync();
    }
}
