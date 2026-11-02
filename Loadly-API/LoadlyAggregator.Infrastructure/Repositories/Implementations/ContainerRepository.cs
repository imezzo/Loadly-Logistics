using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContainerRepository : Repository<Container>, IContainerRepository
{
    public ContainerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Container?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Container?> GetByNameArAsync(string nameAr) =>
        await _dbSet.FirstOrDefaultAsync(c => c.NameAr == nameAr);

    public async Task<Container?> GetByNameEnAsync(string nameEn) =>
        await _dbSet.FirstOrDefaultAsync(c => c.NameEn == nameEn);

    public async Task<Container> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("containers.not_found_with_id", id);

    public async Task<(List<Container> Containers, int TotalCount)> ListAsync(
        string? search,
        ContainerStatus? status,
        int page,
        int pageSize)
    {
        IQueryable<Container> query = _dbSet.AsQueryable().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.NameAr.Contains(search) ||
                c.NameEn.Contains(search));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        int totalCount = await query.CountAsync();
        List<Container> containers = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (containers, totalCount);
    }
}
