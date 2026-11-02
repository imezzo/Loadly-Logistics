using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Invoice?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(i => i.Contract!).ThenInclude(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(i => i.Lead)
            .Include(i => i.Tax)
            .Include(i => i.Discount)
            .Include(i => i.Trips!).ThenInclude(t => t.TripCharges)
            .Include(i => i.Trips!).ThenInclude(t => t.Contract)
            .Include(i => i.Trips!).ThenInclude(t => t.Driver)
            .Include(i => i.ClaimInvoiceTrips!).ThenInclude(t => t.TripCharges)
            .Include(i => i.ClaimInvoiceTrips!).ThenInclude(t => t.Contract)
            .Include(i => i.ClaimInvoiceTrips!).ThenInclude(t => t.Driver)
            .Include(i => i.InvoiceDetail)
            .Include(i => i.Payments)
            .Include(i => i.ContractBillingSnapshot!)
                .ThenInclude(snapshot => snapshot.CalendarDays.Where(day => day.DeletedAt == null))
                    .ThenInclude(day => day.VehicleFacility)
            .Include(i => i.ContractBillingSnapshot!)
                .ThenInclude(snapshot => snapshot.ChargeLines.Where(line => line.DeletedAt == null))
            .Include(i => i.ContractBillingSnapshot!)
                .ThenInclude(snapshot => snapshot.KmEvidence.Where(km => km.DeletedAt == null))
            .AsSplitQuery()
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Invoice> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("invoices.not_found_with_id", id);

    private static IQueryable<Invoice> ApplyListFilters(
        IQueryable<Invoice> query,
        string? search,
        long? companyId,
        long? contractId,
        InvoiceStatus? status,
        InvoiceType? invoiceType,
        DateOnly? startDateFrom,
        DateOnly? startDateTo,
        bool? isOverdue,
        bool? isLeadBilling)
    {
        var businessToday = EgyptTime.ToCalendarDate(DateTime.UtcNow);
        return query
            .Where(i => string.IsNullOrWhiteSpace(search) ||
                i.InvoiceNumber.Contains(search!) ||
                (i.Contract != null && i.Contract.ReferenceNumber.Contains(search!)) ||
                (i.Lead != null && (
                    i.Lead.FullName.Contains(search!) ||
                    i.Lead.Phone.Contains(search!) ||
                    (i.Lead.Email != null && i.Lead.Email.Contains(search!)))))
            .Where(i => !companyId.HasValue
                || isLeadBilling == true
                || (i.Contract != null && i.Contract.CompanyId == companyId.Value))
            .Where(i => !contractId.HasValue || i.ContractId == contractId.Value)
            .Where(i => !isLeadBilling.HasValue || (isLeadBilling.Value ? i.LeadId != null : i.ContractId != null))
            .Where(i => !status.HasValue || i.Status == status.Value)
            .Where(i => !invoiceType.HasValue || i.InvoiceType == invoiceType.Value)
            .Where(i => !startDateFrom.HasValue || i.StartDate >= startDateFrom.Value)
            .Where(i => !startDateTo.HasValue || i.EndDate <= startDateTo.Value)
            .Where(i => !isOverdue.HasValue ||
                (isOverdue.Value
                    ? i.DueDate.HasValue && i.DueDate.Value < businessToday && i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled
                    : !i.DueDate.HasValue || i.DueDate.Value >= businessToday || i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.Cancelled));
    }

    public async Task<(List<Invoice> Invoices, int TotalCount)> ListAsync(
        string? search,
        long? companyId,
        long? contractId,
        InvoiceStatus? status,
        InvoiceType? invoiceType,
        DateOnly? startDateFrom,
        DateOnly? startDateTo,
        bool? isOverdue,
        bool? isLeadBilling,
        int page,
        int pageSize)
    {
        var filtered = ApplyListFilters(_dbSet.AsQueryable(), search, companyId, contractId, status, invoiceType, startDateFrom, startDateTo, isOverdue, isLeadBilling);
        var totalCount = await filtered.CountAsync();
        var invoices = await filtered
            .AsNoTracking()
            .Include(i => i.Contract!).ThenInclude(c => c.Company!).ThenInclude(co => co.CompanyDetail)
            .Include(i => i.Lead)
            .Include(i => i.Tax)
            .Include(i => i.Discount)
            .Include(i => i.Trips!).ThenInclude(t => t.Contract)
            .Include(i => i.ClaimInvoiceTrips!).ThenInclude(t => t.Contract)
            .Include(i => i.Payments)
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (invoices, totalCount);
    }

    private const int InvoiceSequenceLockNamespace = 847291;

    public async Task<int> AllocateNextInvoiceSequenceForYearAsync(int year)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT pg_advisory_xact_lock({0}::integer, {1}::integer)",
            InvoiceSequenceLockNamespace,
            year);

        var maxSequence = await GetMaxInvoiceSequenceForYearAsync(year);
        return (maxSequence ?? 0) + 1;
    }

    public async Task<int?> GetMaxInvoiceSequenceForYearAsync(int year)
    {
        var prefix = $"INV-{year}-";
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(i => i.InvoiceNumber.StartsWith(prefix) && i.Sequence != null)
            .Select(i => i.Sequence)
            .MaxAsync();
    }

    public async Task<List<Invoice>> GetAllForStatisticsAsync() =>
        await _dbSet
            .AsNoTracking()
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .Include(i => i.Payments)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

    public async Task<List<Invoice>> GetAllForStatisticsForCompanyAsync(long companyId) =>
        await _dbSet
            .AsNoTracking()
            .Where(i => i.Status != InvoiceStatus.Cancelled && i.Contract != null && i.Contract.CompanyId == companyId)
            .Include(i => i.Payments)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

    public async Task<int> GetCompanyPaidInvoicesCountInRangeAsync(long companyId, DateTime fromInclusive, DateTime toExclusive) =>
        await _dbSet
            .AsNoTracking()
            .Where(i => i.Contract != null && i.Contract.CompanyId == companyId)
            .Where(i => i.Status == InvoiceStatus.Paid)
            .Where(i => i.UpdatedAt >= fromInclusive && i.UpdatedAt < toExclusive)
            .CountAsync();

    public Task<bool> HasOverlappingTripsInvoiceForContractAsync(
        long contractId,
        DateOnly periodStart,
        DateOnly periodEnd,
        long? excludeInvoiceId = null) =>
        HasOverlappingInvoiceForContractAsync(
            contractId,
            periodStart,
            periodEnd,
            InvoiceType.Trips,
            excludeInvoiceId);

    public Task<bool> HasOverlappingInvoiceForContractAsync(
        long contractId,
        DateOnly periodStart,
        DateOnly periodEnd,
        InvoiceType invoiceType,
        long? excludeInvoiceId = null) =>
        _dbSet
            .AsNoTracking()
            .Where(i => i.DeletedAt == null)
            .Where(i => i.ContractId == contractId)
            .Where(i => i.InvoiceType == invoiceType)
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .Where(i => !excludeInvoiceId.HasValue || i.Id != excludeInvoiceId.Value)
            .Where(i => i.StartDate <= periodEnd && i.EndDate >= periodStart)
            .AnyAsync();
}