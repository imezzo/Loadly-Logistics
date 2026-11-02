using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ServiceRequirementRepository : Repository<ServiceRequirement>, IServiceRequirementRepository
{
    public ServiceRequirementRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<ServiceRequirement?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<ServiceRequirement?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(s => s.NameEn.ToLower() == nameEn.ToLower());

    public async Task<ServiceRequirement> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("service_requirements.not_found_with_id", id);

    public async Task<(List<ServiceRequirement> ServiceRequirements, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(s => string.IsNullOrWhiteSpace(search) ||
                s.NameEn.Contains(search!) ||
                (s.NameAr != null && s.NameAr.Contains(search!)))
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(s => string.IsNullOrWhiteSpace(search) ||
                s.NameEn.Contains(search!) ||
                (s.NameAr != null && s.NameAr.Contains(search!)))
            .CountAsync());

    public override async Task AddAsync(ServiceRequirement serviceRequirement) =>
        await base.AddAsync(serviceRequirement);

    public override void Remove(ServiceRequirement serviceRequirement) =>
        base.Remove(serviceRequirement);

    public override IQueryable<ServiceRequirement> Query() =>
        base.Query();
}

