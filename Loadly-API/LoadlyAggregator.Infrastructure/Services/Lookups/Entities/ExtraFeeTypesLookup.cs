using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class ExtraFeeTypesLookup : Lookup
{
    public ExtraFeeTypesLookup(ILanguageService languageService)
        : base(languageService)
    {
    }

    public override Task<LookupResponse> ExecuteAsync(LookupRequest request)
    {
        var language = LanguageService.GetCurrentLanguage();
        var all = Enum.GetValues<ExtraFeeTypes>()
            .Select(e => new LookupItem
            {
                Id = (long)(int)e,
                Value = language == "ar" ? ExtraFeeTypeDisplayNames.GetNameAr(e) : ExtraFeeTypeDisplayNames.GetNameEn(e),
                Code = e.ToString()
            })
            .ToList();
        var totalCount = all.Count;
        var skip = (request.Page - 1) * request.PageSize;
        var data = all.Skip(skip).Take(request.PageSize).ToList();
        return Task.FromResult(new LookupResponse
        {
            Data = data,
            Pagination = new PaginationDto(request.Page, request.PageSize, totalCount)
        });
    }
}
