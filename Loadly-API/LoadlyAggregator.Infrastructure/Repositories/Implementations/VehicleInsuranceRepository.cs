using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleInsuranceRepository : Repository<VehicleInsurance>, IVehicleInsuranceRepository
{
    public VehicleInsuranceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<VehicleInsurance?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<VehicleInsurance?> GetByPolicyNoAsync(string policyNo) =>
        await _dbSet
            .FirstOrDefaultAsync(v => v.PolicyNo.ToLower() == policyNo.ToLower());

    public async Task<VehicleInsurance> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("vehicle_insurances.not_found_with_id", id);

    public async Task<(List<VehicleInsurance> VehicleInsurances, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.PolicyNo.Contains(search!) ||
                v.InsuranceProvider.Contains(search!))
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.PolicyNo.Contains(search!) ||
                v.InsuranceProvider.Contains(search!))
            .CountAsync());
}

