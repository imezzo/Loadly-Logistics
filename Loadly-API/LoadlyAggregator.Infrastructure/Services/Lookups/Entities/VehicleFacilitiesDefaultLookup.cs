using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class VehicleFacilitiesDefaultLookup : BaseLookup<VehicleFacility>
{
    public VehicleFacilitiesDefaultLookup(
        IVehicleFacilityRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<VehicleFacility, string>> GetValueSelector() =>
        x => x.Name;

    protected override IQueryable<VehicleFacility> ApplyDataSeparation(IQueryable<VehicleFacility> query)
    {
        var companyIds = ParseIdsFromQuery(HttpContextAccessor.HttpContext?.Request.Query, "companyIds", "companyId");
        if (companyIds.Count > 0)
            query = query.Where(v => v.CompanyId.HasValue && companyIds.Contains(v.CompanyId.Value));

        return base.ApplyDataSeparation(query);
    }
}
