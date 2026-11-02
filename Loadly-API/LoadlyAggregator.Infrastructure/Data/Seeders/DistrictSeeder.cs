using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class DistrictSeeder : BaseLookupSeeder<District>
{
    public DistrictSeeder(ApplicationDbContext context) : base(context, "districts.json") { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = JsonSerializer.Deserialize<List<DistrictSeedDto>>(GetJsonContent()) ?? new List<DistrictSeedDto>();
            if (!seedData.Any()) { LogSkipping("Districts (no data)"); return; }

            var cityDict = (await Context.Cities.ToListAsync()).GroupBy(c => c.NameEn).ToDictionary(g => g.Key, g => g.First());
            var existingDistrictDict = (await Context.Districts.ToListAsync()).ToDictionary(d => $"{d.NameEn}|{d.CityId}", d => d);
            var districtsToAdd = new List<District>();
            var districtsToUpdate = new List<District>();
            var processedKeys = new HashSet<string>();
            foreach (var dto in seedData)
            {
                if (!cityDict.TryGetValue(dto.CityNameEn, out var city)) { LogError($"City '{dto.CityNameEn}' not found for district '{dto.NameEn}'"); continue; }
                var key = $"{dto.NameEn}|{city.Id}";
                if (processedKeys.Contains(key)) continue;
                if (existingDistrictDict.TryGetValue(key, out var existing))
                {
                    existing.NameAr = dto.NameAr;
                    districtsToUpdate.Add(existing);
                }
                else
                {
                    districtsToAdd.Add(new District { NameEn = dto.NameEn, NameAr = dto.NameAr, CityId = city.Id });
                }
                processedKeys.Add(key);
            }
            if (districtsToAdd.Any()) { await Context.Districts.AddRangeAsync(districtsToAdd); LogSeeding(districtsToAdd.Count, "districts (added)"); }
            if (districtsToUpdate.Any()) { Context.Districts.UpdateRange(districtsToUpdate); LogSeeding(districtsToUpdate.Count, "districts (updated)"); }
            if (districtsToAdd.Any() || districtsToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding districts: {ex.Message}"); throw; }
    }
    private class DistrictSeedDto
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string CityNameEn { get; set; } = string.Empty;
    }
}
