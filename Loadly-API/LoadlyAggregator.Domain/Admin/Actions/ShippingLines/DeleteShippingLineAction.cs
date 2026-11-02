using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ShippingLines;

public class DeleteShippingLineAction
{
    private readonly IShippingLineRepository _shippingLineRepository;

    public DeleteShippingLineAction(IShippingLineRepository shippingLineRepository)
    {
        _shippingLineRepository = shippingLineRepository;
    }

    public async Task ExecuteAsync(DeleteShippingLineDto dto)
    {
        Common.Entities.ShippingLine shippingLine = await _shippingLineRepository.GetByIdOrThrowAsync(dto.Id);
        _shippingLineRepository.Remove(shippingLine);
        await _shippingLineRepository.SaveChangesAsync();
    }
}
