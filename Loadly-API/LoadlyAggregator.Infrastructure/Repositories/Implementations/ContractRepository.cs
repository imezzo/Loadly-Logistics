using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractRepository : Repository<Contract>, IContractRepository
{
    public ContractRepository(ApplicationDbContext context) : base(context)
    {
    }

    private static IQueryable<Contract> ApplyListFilters(
        IQueryable<Contract> query,
        string? search,
        long? contractId,
        long? companyId,
        ContractType? contractType,
        long? salesRepId,
        ContractStatus? status,
        string? referenceNumber)
    {
        return query
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.NameEn.Contains(search!) ||
                (c.NameAr != null && c.NameAr.Contains(search!)))
            .Where(c => !contractId.HasValue || c.Id == contractId.Value)
            .Where(c => !companyId.HasValue || c.CompanyId == companyId.Value)
            .Where(c => !contractType.HasValue || c.ContractType == contractType.Value)
            .Where(c => !salesRepId.HasValue || c.SalesRepId == salesRepId.Value)
            .Where(c => !status.HasValue || c.Status == status)
            .Where(c => string.IsNullOrWhiteSpace(referenceNumber) || c.ReferenceNumber == referenceNumber!.Trim());
    }

    public override async Task<Contract?> GetByIdAsync(long id) =>
        await _dbSet.AsSplitQuery()
            .Include(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(c => c.SalesRep)
            .Include(c => c.Approvals).ThenInclude(a => a.Approver)
            .Include(c => c.StatusHistories).ThenInclude(h => h.ChangedByAdmin)
            .Include(c => c.ContractVehicleTypes).ThenInclude(cvt => cvt.VehicleType)
            .Include(c => c.ContractDocuments).ThenInclude(cd => cd.Document)
            .Include(c => c.ContractExtraFees)
            .Include(c => c.ContractDestinations).ThenInclude(cd => cd.StartWaypointNavigation)
            .Include(c => c.ContractDestinations).ThenInclude(cd => cd.EndWaypointNavigation)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.VehicleType)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.Vehicle!).ThenInclude(v => v.VehicleType)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.Vehicle!).ThenInclude(v => v.VehicleBrandModel!).ThenInclude(vbm => vbm.VehicleBrand)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.Vehicle!).ThenInclude(v => v.Driver)
            .Include(c => c.Trips).ThenInclude(t => t.TripCharges)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Contract?> GetByNameEnAsync(string nameEn) =>
        await _dbSet
            .Include(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(c => c.SalesRep)
            .FirstOrDefaultAsync(c => c.NameEn.ToLower() == nameEn.ToLower());

    public async Task<Contract?> GetByNameArAsync(string nameAr) =>
        await _dbSet
            .Include(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(c => c.SalesRep)
            .FirstOrDefaultAsync(c => c.NameAr.ToLower() == nameAr.ToLower());

    public async Task<List<Contract>> GetByCompanyIdAsync(long companyId) =>
        await _dbSet
            .Include(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(c => c.SalesRep)
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();

    public async Task<List<Contract>> GetByContractTypeAsync(ContractType contractType) =>
        await _dbSet
            .Include(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(c => c.SalesRep)
            .Where(c => c.ContractType == contractType)
            .ToListAsync();

    public async Task<(List<Contract> Contracts, int TotalCount)> ListAsync(
        string? search,
        long? contractId,
        long? companyId,
        ContractType? contractType,
        long? salesRepId,
        ContractStatus? status,
        string? referenceNumber,
        int page,
        int pageSize)
    {
        var filtered = ApplyListFilters(_dbSet.AsQueryable(), search, contractId, companyId, contractType, salesRepId, status, referenceNumber);
        var totalCount = await filtered.CountAsync();
        var contracts = await filtered
            .AsNoTracking()
            .Include(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(c => c.SalesRep)
            .Include(c => c.Approvals).ThenInclude(a => a.Approver)
            .Include(c => c.StatusHistories).ThenInclude(h => h.ChangedByAdmin)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (contracts, totalCount);
    }

    public async Task<(List<Contract> Contracts, int TotalCount)> ListWithChildrenAsync(
        string? search,
        long? contractId,
        long? companyId,
        ContractType? contractType,
        long? salesRepId,
        ContractStatus? status,
        string? referenceNumber,
        int page,
        int pageSize)
    {
        var filtered = ApplyListFilters(_dbSet.AsNoTracking(), search, contractId, companyId, contractType, salesRepId, status, referenceNumber);
        var totalCount = await filtered.CountAsync();

        var contracts = await filtered
            .Include(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(c => c.SalesRep)
            .Include(c => c.Approvals).ThenInclude(a => a.Approver)
            .Include(c => c.StatusHistories).ThenInclude(h => h.ChangedByAdmin)
            .Include(c => c.ContractVehicleTypes).ThenInclude(cvt => cvt.VehicleType)
            .Include(c => c.ContractDocuments).ThenInclude(cd => cd.Document)
            .Include(c => c.ContractExtraFees)
            .Include(c => c.ContractDestinations).ThenInclude(cd => cd.StartWaypointNavigation)
            .Include(c => c.ContractDestinations).ThenInclude(cd => cd.EndWaypointNavigation)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.VehicleType)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.Vehicle!).ThenInclude(v => v.VehicleType)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.Vehicle!).ThenInclude(v => v.VehicleBrandModel!).ThenInclude(vbm => vbm.VehicleBrand)
            .Include(c => c.ContractSlots).ThenInclude(cs => cs.Vehicle!).ThenInclude(v => v.Driver)
            .Include(c => c.Trips).ThenInclude(t => t.TripCharges)
            .AsSplitQuery()
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (contracts, totalCount);
    }

    public async Task<Contract> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contracts.not_found_with_id", id);

    public async Task<Contract> GetByIdOrThrowForSlotBillingAsync(long id, bool includeContractVehicleTypes = true)
    {
        IQueryable<Contract> query = _dbSet.AsNoTracking()
            .Include(c => c.Company)
                .ThenInclude(company => company.CompanyDetail)
            .Include(c => c.ContractSlots)
                .ThenInclude(cs => cs.VehicleType)
            .Include(c => c.ContractSlots)
                .ThenInclude(cs => cs.Vehicle!)
                    .ThenInclude(v => v.VehicleType)
            .Include(c => c.ContractSlots)
                .ThenInclude(cs => cs.Vehicle!)
                    .ThenInclude(v => v.Driver)
            .Include(c => c.ContractSlots)
                .ThenInclude(cs => cs.Vehicle!)
                    .ThenInclude(v => v.VehicleBrandModel!)
                        .ThenInclude(vbm => vbm.VehicleBrand);

        if (includeContractVehicleTypes)
            query = query.Include(c => c.ContractVehicleTypes);

        return await query
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException("contracts.not_found_with_id", id);
    }

    public async Task<int?> GetMaxReferenceSequenceForYearAsync(int year)
    {
        var startOfYear = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfYear = new DateTime(year, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(c => c.StartDate >= startOfYear && c.StartDate <= endOfYear)
            .Select(c => (int?)c.ReferenceSequence)
            .MaxAsync();
    }

    public async Task<(long CompanyId, ContractStatus Status, ContractConsumptionTypes ConsumptionType)?> GetContractConsumptionTypeAsync(long contractId)
    {
        var row = await _dbSet.AsNoTracking()
            .Where(c => c.Id == contractId)
            .Select(c => new { c.CompanyId, c.Status, c.ConsumptionType })
            .FirstOrDefaultAsync();

        return row is null ? null : (row.CompanyId, row.Status, row.ConsumptionType);
    }
}
