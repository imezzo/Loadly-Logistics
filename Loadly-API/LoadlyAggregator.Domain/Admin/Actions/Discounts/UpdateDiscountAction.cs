using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts;

public class UpdateDiscountAction
{
    private readonly IDiscountRepository _discountRepository;

    public UpdateDiscountAction(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task ExecuteAsync(UpdateDiscountDto dto)
    {
        var discount = await _discountRepository.GetByIdOrThrowAsync(dto.Id);
        discount.NameAr = dto.NameAr;
        discount.NameEn = dto.NameEn;
        discount.Type = dto.Type;
        discount.Value = dto.Value;
        discount.Status = dto.Status;
        await _discountRepository.UpdateAsync(discount);
        await _discountRepository.SaveChangesAsync();
    }
}
