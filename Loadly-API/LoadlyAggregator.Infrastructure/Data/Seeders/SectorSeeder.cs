using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class SectorSeeder : BaseLookupSeeder<Sector>
{
    public SectorSeeder(ApplicationDbContext context) : base(context, null) { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetSectorSeedData();
            if (!seedData.Any()) { LogSkipping("Sectors (no data)"); return; }
            var existingSectorDict = (await Context.Sectors.ToListAsync()).ToDictionary(s => (s.NameEn, s.NameAr), s => s);
            var sectorsToAdd = new List<Sector>();
            var sectorsToUpdate = new List<Sector>();
            var processedKeys = new HashSet<(string, string)>();
            foreach (var sector in seedData)
            {
                var key = (sector.NameEn, sector.NameAr);
                if (processedKeys.Contains(key)) continue;
                if (existingSectorDict.TryGetValue(key, out var existing))
                {
                    existing.NameEn = sector.NameEn;
                    existing.NameAr = sector.NameAr;
                    sectorsToUpdate.Add(existing);
                }
                else
                {
                    sectorsToAdd.Add(sector);
                }
                processedKeys.Add(key);
            }
            if (sectorsToAdd.Any()) { await Context.Sectors.AddRangeAsync(sectorsToAdd); LogSeeding(sectorsToAdd.Count, "sectors (added)"); }
            if (sectorsToUpdate.Any()) { Context.Sectors.UpdateRange(sectorsToUpdate); LogSeeding(sectorsToUpdate.Count, "sectors (updated)"); }
            if (sectorsToAdd.Any() || sectorsToUpdate.Any()) await Context.SaveChangesAsync();
            else LogSkipping("Sectors (no changes)");
        }
        catch (Exception ex) { LogError($"Error seeding sectors: {ex.Message}"); throw; }
    }
    private static List<Sector> GetSectorSeedData() => new()
    {
        new() { NameEn = "Fresh Mixed Sector", NameAr = "قطاع متنوع فرش" },
        new() { NameEn = "Mixed Box Sector", NameAr = "قطاع متنوع صندوق" },
        new() { NameEn = "Chipsy Box Sector", NameAr = "قطاع صندوق شيبسي" },
        new() { NameEn = "Port Sector", NameAr = "قطاع المينا" },
        new() { NameEn = "Jumbo Sector", NameAr = "قطاع الجامبو" }
    };
}
