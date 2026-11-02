using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class WaypointsDefaultLookup : BaseLookup<Waypoint>
{
    private readonly IRepository<ContractDestination> _contractDestinationRepository;

    public WaypointsDefaultLookup(
        IWaypointRepository repository,
        IRepository<ContractDestination> contractDestinationRepository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
        _contractDestinationRepository = contractDestinationRepository;
    }

    protected override Expression<Func<Waypoint, string>> GetValueSelector() =>
        x => x.AddressName ?? string.Empty;

    protected override IQueryable<Waypoint> ApplyDataSeparation(IQueryable<Waypoint> query)
    {
        var contractIdValue = HttpContextAccessor.HttpContext?.Request?.Query["contractId"].ToString();

        if (long.TryParse(contractIdValue, out var contractId))
        {
            query = query.Where(w => _contractDestinationRepository
                .Query()
                .Any(cd =>
                    cd.ContractId == contractId
                    && cd.Status != ContractDestinationStatus.ARCHIVED
                    && (cd.StartWaypointId == w.Id || cd.EndWaypointId == w.Id)));
        }

        return base.ApplyDataSeparation(query);
    }
}
