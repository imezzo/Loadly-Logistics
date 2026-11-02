using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts;

public class DeleteDiscountAction
{
    private readonly IDiscountRepository _discountRepository;

    public DeleteDiscountAction(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task ExecuteAsync(DeleteDiscountDto dto)
    {
        var discount = await _discountRepository.GetByIdOrThrowAsync(dto.Id);
        _discountRepository.Remove(discount);
        await _discountRepository.SaveChangesAsync();
    }
}
