using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class VehicleBrandModelsDefaultLookup : BaseLookup<VehicleBrandModel>
{
    public VehicleBrandModelsDefaultLookup(
        IVehicleBrandModelRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<VehicleBrandModel, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? x => x.NameAr ?? x.NameEn ?? string.Empty
            : x => x.NameEn ?? x.NameAr ?? string.Empty;
    }

    protected override IQueryable<VehicleBrandModel> ApplyDataSeparation(IQueryable<VehicleBrandModel> query)
    {
        var brandIdValue = HttpContextAccessor.HttpContext?.Request?.Query["brandId"].ToString();

        if (long.TryParse(brandIdValue, out var brandId))
        {
            query = query.Where(m => m.VehicleBrandId == brandId);
        }

        return base.ApplyDataSeparation(query);
    }
}

