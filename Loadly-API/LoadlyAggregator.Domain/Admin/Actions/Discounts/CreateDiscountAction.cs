using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Discounts;

public class CreateDiscountAction
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateDiscountAction(
        IDiscountRepository discountRepository,
        ICurrentUserService currentUserService)
    {
        _discountRepository = discountRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateDiscountDto dto)
    {
        await _discountRepository.AddAsync(new Discount
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Type = dto.Type,
            Value = dto.Value,
            Status = dto.Status,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _discountRepository.SaveChangesAsync();
    }
}
