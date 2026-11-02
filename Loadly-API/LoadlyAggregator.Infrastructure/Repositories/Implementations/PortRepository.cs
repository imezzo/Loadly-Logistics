using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class PortRepository : Repository<Port>, IPortRepository
{
    public PortRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Port?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Port?> GetByNameArCountryAndTypeAsync(string nameAr, string country, PortType type) =>
        await _dbSet.FirstOrDefaultAsync(p => p.NameAr == nameAr && p.Country == country && p.Type == type);

    public async Task<Port?> GetByPortCodeAsync(string portCode) =>
        await _dbSet.FirstOrDefaultAsync(p => p.PortCode == portCode);

    public async Task<Port> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("ports.not_found_with_id", id);

    public async Task<(List<Port> Ports, int TotalCount)> ListAsync(
        string? search,
        PortType? type,
        PortStatus? status,
        int page,
        int pageSize)
    {
        IQueryable<Port> query = _dbSet.AsQueryable().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.NameAr.Contains(search) ||
                (p.NameEn != null && p.NameEn.Contains(search)) ||
                p.Country.Contains(search) ||
                (p.PortCode != null && p.PortCode.Contains(search)));
        }

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        int totalCount = await query.CountAsync();
        List<Port> ports = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (ports, totalCount);
    }
}
