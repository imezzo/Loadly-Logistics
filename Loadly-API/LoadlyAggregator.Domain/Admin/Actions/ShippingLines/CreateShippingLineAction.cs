using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ShippingLines;

public class CreateShippingLineAction
{
    private readonly IShippingLineRepository _shippingLineRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateShippingLineAction(
        IShippingLineRepository shippingLineRepository,
        ICurrentUserService currentUserService)
    {
        _shippingLineRepository = shippingLineRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateShippingLineDto dto)
    {
        await _shippingLineRepository.AddAsync(new ShippingLine
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Status = dto.Status,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _shippingLineRepository.SaveChangesAsync();
    }
}
