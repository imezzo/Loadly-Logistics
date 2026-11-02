using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class LeadRepository : Repository<Lead>, ILeadRepository
{
    public LeadRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Lead?> GetByPhoneAsync(string phone) =>
        await _dbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Phone == phone);

    public async Task<(List<Lead> Leads, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await Query()
            .Where(l =>
                string.IsNullOrWhiteSpace(search)
                || l.FullName.Contains(search!)
                || l.Phone.Contains(search!)
                || (l.Email != null && l.Email.Contains(search!)))
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await Query()
            .Where(l =>
                string.IsNullOrWhiteSpace(search)
                || l.FullName.Contains(search!)
                || l.Phone.Contains(search!)
                || (l.Email != null && l.Email.Contains(search!)))
            .CountAsync());

    public async Task<Lead> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("leads.not_found_with_id", id);

    public async Task<bool> ExistsByPhoneAsync(string phone, long excludeId) =>
        await _dbSet.AnyAsync(l => l.Phone == phone && l.Id != excludeId);
    public async Task<Lead?> GetActiveByPhoneAsync(string phone) =>
        await Query().FirstOrDefaultAsync(l => l.Phone == phone);

    public async Task<long> UpsertByPhoneAsync(string fullName, string phone, string? email, long? addedBy)
    {
        var existing = await GetByPhoneAsync(phone);
        if (existing is not null)
        {
            existing.FullName = fullName;
            if (email is not null)
                existing.Email = email;
            existing.DeletedAt = null;
            Update(existing);
            await SaveChangesAsync();
            return existing.Id;
        }

        var lead = new Lead
        {
            FullName = fullName,
            Phone = phone,
            Email = email,
            AddedBy = addedBy
        };
        await AddAsync(lead);
        await SaveChangesAsync();
        return lead.Id;
    }

    public async Task<LeadAggregateCounts> GetAggregateCountsAsync(long leadId) =>
        new(await GetCompletedTripCountQuery(leadId).CountAsync());

    public async Task<Dictionary<long, LeadAggregateCounts>> GetAggregateCountsByIdsAsync(IReadOnlyList<long> leadIds) =>
        await _dbSet
            .AsNoTracking()
            .Where(l => leadIds.Contains(l.Id) && l.DeletedAt == null)
            .Select(l => new
            {
                l.Id,
                Counts = new LeadAggregateCounts(
                    l.Trips.Count(t => t.Status == TripStatus.Completed))
            })
            .ToDictionaryAsync(x => x.Id, x => x.Counts);

    private IQueryable<Trip> GetCompletedTripCountQuery(long leadId) =>
        _context.Trips
            .AsNoTracking()
            .Where(t => t.LeadId == leadId)
            .Where(t => t.Status == TripStatus.Completed);
}
