namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IServiceRequirementRepository : IRepository<Common.Entities.ServiceRequirement>
{
    Task<Common.Entities.ServiceRequirement?> GetByNameAsync(string nameEn);
    Task<Common.Entities.ServiceRequirement> GetByIdOrThrowAsync(long id);
    Task<(List<Common.Entities.ServiceRequirement> ServiceRequirements, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
}
