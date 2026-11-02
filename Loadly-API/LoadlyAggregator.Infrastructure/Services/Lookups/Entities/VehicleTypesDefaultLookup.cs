using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class VehicleTypesDefaultLookup : BaseLookup<VehicleType>
{
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;

    public VehicleTypesDefaultLookup(
        IVehicleTypeRepository repository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
    }

    protected override Expression<Func<VehicleType, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? x => x.NameAr ?? x.NameEn ?? string.Empty
            : x => x.NameEn ?? x.NameAr ?? string.Empty;
    }

    protected override IQueryable<VehicleType> ApplyDataSeparation(IQueryable<VehicleType> query)
    {
        var contractId = GetContractIdFromQuery();
        return base.ApplyDataSeparation(contractId.HasValue ? query.Where(vt => _contractVehicleTypeRepository.Query()
            .Any(cvt => cvt.ContractId == contractId.Value && cvt.VehicleTypeId == vt.Id)) : query);
    }

    private long? GetContractIdFromQuery()
    {
        var value = HttpContextAccessor.HttpContext?.Request?.Query["contractId"].ToString();
        return long.TryParse(value, out var id) ? id : null;
    }
}
