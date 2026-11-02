using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class DocumentStatusSeeder : BaseLookupSeeder<LookupDocumentStatus>
{
    public DocumentStatusSeeder(ApplicationDbContext context) : base(context, null) { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetDocumentStatusSeedData();
            if (!seedData.Any()) { LogSkipping("Document statuses (no data)"); return; }
            var existingStatusDict = (await Context.LookupDocumentStatuses.ToListAsync()).ToDictionary(d => (d.NameEn, d.NameAr), d => d);
            var statusesToAdd = new List<LookupDocumentStatus>();
            var statusesToUpdate = new List<LookupDocumentStatus>();
            var processedKeys = new HashSet<(string, string)>();
            foreach (var documentStatus in seedData)
            {
                var key = (documentStatus.NameEn, documentStatus.NameAr);
                if (processedKeys.Contains(key)) continue;
                if (existingStatusDict.TryGetValue(key, out var existing))
                {
                    existing.NameEn = documentStatus.NameEn;
                    existing.NameAr = documentStatus.NameAr;
                    statusesToUpdate.Add(existing);
                }
                else
                {
                    statusesToAdd.Add(documentStatus);
                }
                processedKeys.Add(key);
            }
            if (statusesToAdd.Any()) { await Context.LookupDocumentStatuses.AddRangeAsync(statusesToAdd); LogSeeding(statusesToAdd.Count, "document statuses (added)"); }
            if (statusesToUpdate.Any()) { Context.LookupDocumentStatuses.UpdateRange(statusesToUpdate); LogSeeding(statusesToUpdate.Count, "document statuses (updated)"); }
            if (statusesToAdd.Any() || statusesToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding document statuses: {ex.Message}"); throw; }
    }
    private static List<LookupDocumentStatus> GetDocumentStatusSeedData() => new()
    {
        new() { NameEn = "Pending", NameAr = "قيد الإنتظار" },
        new() { NameEn = "Expired", NameAr = "منتهي الصلاحية" },
        new() { NameEn = "Archived", NameAr = "مؤرشف" },
        new() { NameEn = "Verified", NameAr = "موثق" },
        new() { NameEn = "Rejected", NameAr = "مرفوض" }
    };
}
