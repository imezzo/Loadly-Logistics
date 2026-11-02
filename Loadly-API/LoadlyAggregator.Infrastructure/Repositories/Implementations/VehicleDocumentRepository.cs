using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleDocumentRepository : Repository<VehicleDocument>, IVehicleDocumentRepository
{
    public VehicleDocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<VehicleDocument?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(vd => vd.Document)
            .Include(vd => vd.Vehicle)
            .FirstOrDefaultAsync(vd => vd.Id == id);

    public override IQueryable<VehicleDocument> Query() =>
        base.Query()
            .Include(vd => vd.Document)
            .Include(vd => vd.Vehicle);

    public override async Task AddAsync(VehicleDocument vehicleDocument) =>
        await base.AddAsync(vehicleDocument);

    public override void Remove(VehicleDocument vehicleDocument) =>
        base.Remove(vehicleDocument);

    public async Task<VehicleDocument> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("vehicle_documents.not_found_with_id", id);
    public async Task<IEnumerable<VehicleDocument>> GetByVehicleIdAsync(long vehicleId) =>
        await _dbSet
            .Include(vd => vd.Document)
            .Where(vd => vd.VehicleId == vehicleId)
            .ToListAsync();
    public async Task<(List<VehicleDocument> VehicleDocuments, int TotalCount)> ListAsync(
        long? vehicleId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        int page = 1,
        int pageSize = 10) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Include(vd => vd.Document)
            .Include(vd => vd.Vehicle)
            .Where(vd => !vehicleId.HasValue || vd.VehicleId == vehicleId.Value)
            .Where(vd => !documentId.HasValue || vd.DocumentId == documentId.Value)
            .Where(vd => !status.HasValue || vd.Status == status.Value)
            .OrderByDescending(vd => vd.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Include(vd => vd.Document)
            .Include(vd => vd.Vehicle)
            .Where(vd => !vehicleId.HasValue || vd.VehicleId == vehicleId.Value)
            .Where(vd => !documentId.HasValue || vd.DocumentId == documentId.Value)
            .Where(vd => !status.HasValue || vd.Status == status.Value)
            .CountAsync());
}
