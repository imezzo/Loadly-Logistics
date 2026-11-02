using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class VehicleCapacityUnitsDefaultLookup : BaseLookup<VehicleCapacityUnit>
{
    public VehicleCapacityUnitsDefaultLookup(
        IVehicleCapacityUnitRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<VehicleCapacityUnit, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? x => x.CapacityTypeAr ?? x.CapacityTypeEn ?? string.Empty
            : x => x.CapacityTypeEn ?? x.CapacityTypeAr ?? string.Empty;
    }
}

