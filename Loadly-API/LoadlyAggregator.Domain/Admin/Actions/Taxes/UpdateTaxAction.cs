using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Taxes;

public class UpdateTaxAction
{
    private readonly ITaxRepository _taxRepository;

    public UpdateTaxAction(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository;
    }

    public async Task ExecuteAsync(UpdateTaxDto dto)
    {
        var tax = await _taxRepository.GetByIdOrThrowAsync(dto.Id);
        tax.NameAr = dto.NameAr;
        tax.NameEn = dto.NameEn;
        tax.Type = dto.Type;
        tax.Value = dto.Value;
        tax.Status = dto.Status;
        await _taxRepository.UpdateAsync(tax);
        await _taxRepository.SaveChangesAsync();
    }
}
