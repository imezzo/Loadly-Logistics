using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContactRepository : IRepository<Contact>
{
    Task<IEnumerable<Contact>> GetByModelAsync(string modelType, long modelId);
    Task<List<Contact>> GetByModelAndModelIdsAsync(string modelType, IEnumerable<long> modelIds);
    Task<(IEnumerable<Contact> Contacts, int TotalCount)> SearchAsync(
        string? search = null,
        string? modelType = null,
        long? modelId = null,
        bool? primary = null,
        long? communicationMethodId = null,
        int page = 1,
        int pageSize = 10);

    Task<(List<Contact> Contacts, int TotalCount)> ListAsync(
        string? search,
        string? modelType,
        long? modelId,
        bool? primary,
        long? communicationMethodId,
        int page,
        int pageSize);

    Task<Contact> GetByIdOrThrowAsync(long id);
}
