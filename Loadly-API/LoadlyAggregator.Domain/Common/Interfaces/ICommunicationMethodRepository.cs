namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ICommunicationMethodRepository : IRepository<Common.Entities.CommunicationMethod>
{
    Task<Common.Entities.CommunicationMethod?> GetByNameAsync(string nameEn);
    Task<Common.Entities.CommunicationMethod> GetByIdOrThrowAsync(long id);
    Task<(List<Common.Entities.CommunicationMethod> CommunicationMethods, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
}
