using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Tag?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Tag?> GetByNameEnAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(t => t.NameEn.ToLower() == nameEn.ToLower());

    public async Task<(List<Tag> Tags, int TotalCount)> ListAsync(
        string? search,
        bool? isPricing,
        int page,
        int pageSize)
    {
        var query = _dbSet.AsQueryable().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.NameEn.Contains(search) ||
                (t.NameAr != null && t.NameAr.Contains(search)));
        }

        if (isPricing.HasValue)
        {
            query = query.Where(t => t.IsPricing == isPricing.Value);
        }

        var totalCount = await query.CountAsync();
        var tags = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (tags, totalCount);
    }

    public async Task<Tag> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("tags.not_found_with_id", id);
}
