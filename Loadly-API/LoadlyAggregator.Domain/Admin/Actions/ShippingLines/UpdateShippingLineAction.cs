using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ShippingLines;

public class UpdateShippingLineAction
{
    private readonly IShippingLineRepository _shippingLineRepository;

    public UpdateShippingLineAction(IShippingLineRepository shippingLineRepository)
    {
        _shippingLineRepository = shippingLineRepository;
    }

    public async Task ExecuteAsync(UpdateShippingLineDto dto)
    {
        Common.Entities.ShippingLine shippingLine = await _shippingLineRepository.GetByIdOrThrowAsync(dto.Id);
        shippingLine.NameAr = dto.NameAr;
        shippingLine.NameEn = dto.NameEn;
        shippingLine.Status = dto.Status;
        await _shippingLineRepository.UpdateAsync(shippingLine);
        await _shippingLineRepository.SaveChangesAsync();
    }
}
