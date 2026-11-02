using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DiscountRepository : Repository<Discount>, IDiscountRepository
{
    public DiscountRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Discount?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Discount?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(d => d.NameEn.ToLower() == nameEn.ToLower());

    public override async Task AddAsync(Discount discount) =>
        await base.AddAsync(discount);

    public override void Remove(Discount discount) =>
        base.Remove(discount);

    public override IQueryable<Discount> Query() =>
        base.Query();

    public async Task<(List<Discount> Discounts, int TotalCount)> ListAsync(
        string? search,
        DiscountType? type,
        DiscountStatus? status,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(d => string.IsNullOrWhiteSpace(search) ||
                d.NameEn.Contains(search!) ||
                (d.NameAr != null && d.NameAr.Contains(search!)))
            .Where(d => !type.HasValue || d.Type == type.Value)
            .Where(d => !status.HasValue || d.Status == status.Value)
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(d => string.IsNullOrWhiteSpace(search) ||
                d.NameEn.Contains(search!) ||
                (d.NameAr != null && d.NameAr.Contains(search!)))
            .Where(d => !type.HasValue || d.Type == type.Value)
            .Where(d => !status.HasValue || d.Status == status.Value)
            .CountAsync());

    public async Task<Discount> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("discounts.not_found_with_id", id);
}

