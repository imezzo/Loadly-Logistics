using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    public ContactRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Contact?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<Contact>> GetByModelAsync(string modelType, long modelId) =>
        await _dbSet
            .Where(c => c.ModelType == modelType && c.ModelId == modelId)
            .ToListAsync();

    public async Task<List<Contact>> GetByModelAndModelIdsAsync(string modelType, IEnumerable<long> modelIds)
    {
        var ids = modelIds.ToList();
        if (ids.Count == 0)
            return new List<Contact>();
        return await _dbSet
            .Where(c => c.ModelType == modelType && ids.Contains(c.ModelId))
            .ToListAsync();
    }

    public async Task<(IEnumerable<Contact> Contacts, int TotalCount)> SearchAsync(
        string? search = null,
        string? modelType = null,
        long? modelId = null,
        bool? primary = null,
        long? communicationMethodId = null,
        int page = 1,
        int pageSize = 10) =>
        (await BuildSearchQuery(search, modelType, modelId, primary, communicationMethodId)
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await BuildSearchQuery(search, modelType, modelId, primary, communicationMethodId).CountAsync());

    private IQueryable<Contact> BuildSearchQuery(string? search, string? modelType, long? modelId, bool? primary, long? communicationMethodId) =>
        _dbSet.AsQueryable()
            .Where(c => string.IsNullOrWhiteSpace(search) || c.Name.Contains(search) || (c.Email != null && c.Email.Contains(search)) || (c.Phone != null && c.Phone.Contains(search)) || (c.Position != null && c.Position.Contains(search)))
            .Where(c => string.IsNullOrWhiteSpace(modelType) || c.ModelType == modelType)
            .Where(c => !modelId.HasValue || c.ModelId == modelId.Value)
            .Where(c => !primary.HasValue || c.Primary == primary.Value)
            .Where(c => !communicationMethodId.HasValue || c.CommunicationMethodId == communicationMethodId.Value);

    public async Task<(List<Contact> Contacts, int TotalCount)> ListAsync(
        string? search,
        string? modelType,
        long? modelId,
        bool? primary,
        long? communicationMethodId,
        int page,
        int pageSize) =>
        (await BuildSearchQuery(search, modelType, modelId, primary, communicationMethodId)
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await BuildSearchQuery(search, modelType, modelId, primary, communicationMethodId).CountAsync());

    public async Task<Contact> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("contacts.not_found_with_id", id);
}

