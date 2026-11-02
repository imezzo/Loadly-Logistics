using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Taxes;

public class CreateTaxAction
{
    private readonly ITaxRepository _taxRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateTaxAction(ITaxRepository taxRepository, ICurrentUserService currentUserService)
    {
        _taxRepository = taxRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateTaxDto dto)
    {
        await _taxRepository.AddAsync(new Tax
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Type = dto.Type,
            Value = dto.Value,
            Status = dto.Status,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _taxRepository.SaveChangesAsync();
    }
}
