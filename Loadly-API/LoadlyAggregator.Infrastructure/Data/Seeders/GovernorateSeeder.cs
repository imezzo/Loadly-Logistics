using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class GovernorateSeeder : BaseLookupSeeder<Governerate>
{
    public GovernorateSeeder(ApplicationDbContext context) : base(context, "governerates.json") { }
    public override async Task SeedAsync()
    {
        try
        {
            var jsonContent = GetJsonContent();
            if (string.IsNullOrWhiteSpace(jsonContent)) { LogSkipping("Governerates (no data)"); return; }
            var seedData = JsonSerializer.Deserialize<List<Governerate>>(jsonContent) ?? new List<Governerate>();
            if (!seedData.Any()) { LogSkipping("Governerates (no data)"); return; }
            
            var existingGovernorates = await Context.Governerates.ToListAsync();
            var existingByNameEn = existingGovernorates.ToDictionary(g => g.NameEn, g => g);
            var existingByNameAr = existingGovernorates.ToDictionary(g => g.NameAr, g => g);
            var governoratesToAdd = new List<Governerate>();
            var governoratesToUpdate = new List<Governerate>();
            var processedNamesEn = new HashSet<string>();
            foreach (var gov in seedData)
            {
                var nameEn = gov.NameEn ?? string.Empty;
                var nameAr = gov.NameAr ?? string.Empty;
                if (string.IsNullOrEmpty(nameEn) || string.IsNullOrEmpty(nameAr) || processedNamesEn.Contains(nameEn)) continue;
                processedNamesEn.Add(nameEn);
                Governerate? existing = existingByNameEn.TryGetValue(nameEn, out var byEn) ? byEn :
                                        existingByNameAr.TryGetValue(nameAr, out var byAr) ? byAr : null;
                if (existing != null)
                {
                    existing.NameEn = nameEn;
                    existing.NameAr = nameAr;
                    governoratesToUpdate.Add(existing);
                }
                else
                {
                    governoratesToAdd.Add(new Governerate { NameEn = nameEn, NameAr = nameAr });
                }
            }
            if (governoratesToAdd.Any()) { await Context.Governerates.AddRangeAsync(governoratesToAdd); LogSeeding(governoratesToAdd.Count, "governerates (added)"); }
            if (governoratesToUpdate.Any()) { Context.Governerates.UpdateRange(governoratesToUpdate); LogSeeding(governoratesToUpdate.Count, "governerates (updated)"); }
            if (governoratesToAdd.Any() || governoratesToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding governerates: {ex.Message}"); throw; }
    }
}
