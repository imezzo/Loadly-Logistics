using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class ContractVehicleTypesDefaultLookup : BaseLookup<ContractVehicleType>
{
    public ContractVehicleTypesDefaultLookup(
        IContractVehicleTypeRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<ContractVehicleType, long>> GetIdSelector() =>
        cvt => cvt.VehicleTypeId;

    protected override Expression<Func<ContractVehicleType, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? cvt => cvt.VehicleType != null
                ? (cvt.VehicleType.NameAr ?? cvt.VehicleType.NameEn ?? string.Empty)
                : string.Empty
            : cvt => cvt.VehicleType != null
                ? (cvt.VehicleType.NameEn ?? cvt.VehicleType.NameAr ?? string.Empty)
                : string.Empty;
    }

    protected override IQueryable<ContractVehicleType> ApplyDataSeparation(IQueryable<ContractVehicleType> query)
    {
        return query;
    }
}
