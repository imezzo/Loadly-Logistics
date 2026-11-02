using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class VehicleInsurancesDefaultLookup : BaseLookup<VehicleInsurance>
{
    public VehicleInsurancesDefaultLookup(
        IVehicleInsuranceRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<VehicleInsurance, string>> GetValueSelector()
    {
        return x => (x.PolicyNo ?? string.Empty) + " - " + (x.InsuranceProvider ?? string.Empty);
    }
}

