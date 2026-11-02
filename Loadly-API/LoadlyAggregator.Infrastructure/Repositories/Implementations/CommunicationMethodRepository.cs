using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class CommunicationMethodRepository : Repository<CommunicationMethod>, ICommunicationMethodRepository
{
    public CommunicationMethodRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<CommunicationMethod?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<CommunicationMethod?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(c => c.NameEn.ToLower() == nameEn.ToLower());

    public async Task<CommunicationMethod> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("communication_methods.not_found_with_id", id);

    public async Task<(List<CommunicationMethod> CommunicationMethods, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.NameEn.Contains(search!) ||
                (c.NameAr != null && c.NameAr.Contains(search!)))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.NameEn.Contains(search!) ||
                (c.NameAr != null && c.NameAr.Contains(search!)))
            .CountAsync());

    public override async Task AddAsync(CommunicationMethod communicationMethod) =>
        await base.AddAsync(communicationMethod);

    public override void Remove(CommunicationMethod communicationMethod) =>
        base.Remove(communicationMethod);

    public override IQueryable<CommunicationMethod> Query() =>
        base.Query();
}

