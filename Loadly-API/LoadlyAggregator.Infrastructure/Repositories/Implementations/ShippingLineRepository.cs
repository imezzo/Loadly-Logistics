using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ShippingLineRepository : Repository<ShippingLine>, IShippingLineRepository
{
    public ShippingLineRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<ShippingLine?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<ShippingLine?> GetByNameArAsync(string nameAr) =>
        await _dbSet.FirstOrDefaultAsync(s => s.NameAr == nameAr);

    public async Task<ShippingLine?> GetByNameEnAsync(string nameEn) =>
        await _dbSet.FirstOrDefaultAsync(s => s.NameEn == nameEn);

    public async Task<ShippingLine> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("shipping_lines.not_found_with_id", id);

    public async Task<(List<ShippingLine> ShippingLines, int TotalCount)> ListAsync(
        string? search,
        ShippingLineStatus? status,
        int page,
        int pageSize)
    {
        IQueryable<ShippingLine> query = _dbSet.AsQueryable().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.NameAr.Contains(search) ||
                s.NameEn.Contains(search));
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        int totalCount = await query.CountAsync();
        List<ShippingLine> shippingLines = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (shippingLines, totalCount);
    }
}
